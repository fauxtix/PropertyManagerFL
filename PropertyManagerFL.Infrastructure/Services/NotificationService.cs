using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.Notifications;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Infrastructure.Services;

/// <summary>
/// Server-side notification operations and the single rules engine for operational alerts.
/// Alerts are materialised into the user's inbox and are also consumed by the Arrendamentos
/// page, which only formats the localized text.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly IAppointmentRepository _appointments;
    private readonly IAppSettingsRepository _appSettings;
    private readonly IArrendamentoRepository _leases;
    private readonly IInquilinoRepository _tenants;
    private readonly IRecebimentoRepository _payments;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notifications,
        IAppointmentRepository appointments,
        IAppSettingsRepository appSettings,
        IArrendamentoRepository leases,
        IInquilinoRepository tenants,
        IRecebimentoRepository payments,
        ILogger<NotificationService> logger)
    {
        _notifications = notifications;
        _appointments = appointments;
        _appSettings = appSettings;
        _leases = leases;
        _tenants = tenants;
        _payments = payments;
        _logger = logger;
    }

    public async Task<int> NotifyAsync(CreateNotificationVM notification)
    {
        if (string.IsNullOrWhiteSpace(notification.RecipientUserId) ||
            string.IsNullOrWhiteSpace(notification.Kind) ||
            string.IsNullOrWhiteSpace(notification.Title))
            return -1;

        return await _notifications.InsertAsync(new Notification
        {
            RecipientUserId = notification.RecipientUserId,
            TenantId = notification.TenantId,
            ActorUserId = notification.ActorUserId,
            Kind = notification.Kind,
            Title = notification.Title,
            Body = notification.Body ?? string.Empty,
            Link = notification.Link,
            Severity = string.IsNullOrWhiteSpace(notification.Severity) ? "Info" : notification.Severity,
            SuggestedAction = notification.SuggestedAction,
            ResourceType = notification.ResourceType,
            ResourceId = notification.ResourceId,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<IEnumerable<NotificationVM>> GetForUserAsync(string recipientUserId, int take = 100)
    {
        var items = await _notifications.GetForUserAsync(recipientUserId, take);
        return items.Select(n => new NotificationVM
        {
            Id = n.Id,
            Kind = n.Kind,
            Title = n.Title,
            Body = n.Body,
            Link = n.Link,
            Severity = n.Severity,
            SuggestedAction = n.SuggestedAction,
            ResourceType = n.ResourceType,
            ResourceId = n.ResourceId,
            CreatedAt = n.CreatedAt,
            IsRead = n.ReadAt.HasValue
        }).ToList();
    }

    public async Task<NotificationCountVM> GetCountAsync(string recipientUserId)
        => await _notifications.GetCountAsync(recipientUserId);

    public async Task<bool> MarkReadAsync(int id, string recipientUserId)
        => await _notifications.MarkReadAsync(id, recipientUserId);

    public async Task<int> MarkAllReadAsync(string recipientUserId)
        => await _notifications.MarkAllReadAsync(recipientUserId);

    public async Task<int> MarkSeenAsync(string recipientUserId)
        => await _notifications.MarkSeenAsync(recipientUserId);

    /// <summary>
    /// Computes the operational alerts shown in the bell and on the Arrendamentos page.
    /// Best-effort: any data-source failure is logged and the remaining rules still run.
    /// </summary>
    public async Task<IEnumerable<NotificationAlertVM>> GetAlertsAsync()
    {
        var alerts = new List<NotificationAlertVM>();
        try
        {
            var settings = await _appSettings.GetSettingsAsync();
            var leases = (await _leases.GetAll() ?? Enumerable.Empty<ArrendamentoVM>()).ToList();

            // Shared snapshots. Each source is optional: a failure must not suppress unrelated rules.
            var payments = new List<RecebimentoVM>();
            var tenantDocuments = new List<DocumentoInquilinoVM>();
            try { payments = (await _payments.GetAll() ?? Enumerable.Empty<RecebimentoVM>()).ToList(); }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'payments snapshot' failed"); }
            try { tenantDocuments = (await _tenants.GetDocumentos() ?? Enumerable.Empty<DocumentoInquilinoVM>()).ToList(); }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'tenant documents snapshot' failed"); }

            // Appointments today
            try
            {
                var appts = await _appointments.GetAllAsync();
                if (appts is not null)
                {
                    foreach (var appt in appts.Where(a => a.StartTime.Date == DateTime.Today))
                    {
                        var notes = string.IsNullOrWhiteSpace(appt.Description) ? "" : $" ({appt.Description})";
                        alerts.Add(new NotificationAlertVM
                        {
                            Kind = "Appointment",
                            Severity = "Info",
                            Title = appt.Subject,
                            Body = $"{appt.Location} ({appt.StartTime:HH:mm}){notes}".Trim(),
                            Link = "/calendar",
                            ResourceType = "Appointment",
                            ResourceId = appt.Id
                        });
                    }
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'appointments' failed"); }

            if (settings is not null && !settings.CartasAumentoAutomaticas)
            {
                // Rent update letters - manual mode
                try
                {
                    var docs = tenantDocuments;
                    if (docs is not null)
                    {
                        // BUG FIX: only documents of type 16 (rent-update letter) from previous years
                        // trigger the alert; previously every document was reported.
                        var pending = docs.Where(d => d.DocumentType == 16 && d.CreationDate.Year < DateTime.Today.Year).ToList();
                        foreach (var doc in pending)
                        {
                            alerts.Add(new NotificationAlertVM
                            {
                                Kind = "RentLetter",
                                Severity = "Success",
                                Title = "Necessário envio de carta de atualização",
                                Body = doc.NomeInquilino,
                                Link = "/tenants",
                                ResourceType = "TenantDocument",
                                ResourceId = doc.Id,
                                PrimaryText = doc.NomeInquilino
                            });
                        }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'rent letters' failed"); }

                // BUG FIX: use AddMonths so December + 1 is January of the next year, never month 13.
                var nextMonth = DateTime.Today.AddMonths(1);
                var leasesToUpdate = leases.Where(l => l.Data_Inicio.Month == nextMonth.Month &&
                                                       l.Data_Inicio.Year == nextMonth.Year).ToList();
                foreach (var lease in leasesToUpdate)
                {
                    var suffix = lease.EnvioCartaAtualizacaoRenda ? "" : " - não foi enviada carta de atualização!";
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "RentUpdate",
                        Severity = "Success",
                        Title = $"Necessário atualizar renda do inquilino {lease.NomeInquilino} ({lease.Fracao})",
                        Body = suffix.TrimStart(' ', '-'),
                        Link = "/arrendamentos",
                        ResourceType = "Lease",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao,
                        NeedsAction = !lease.EnvioCartaAtualizacaoRenda
                    });
                }

                if (!leasesToUpdate.Any())
                {
                    try
                    {
                        if (leases.Any() && payments.Any() &&
                            !await _leases.CartaAtualizacaoRendasEmitida(DateTime.Today.Year))
                        {
                            alerts.Add(new NotificationAlertVM
                            {
                                Kind = "RentLetters",
                                Severity = "Error",
                                Title = "Cartas de atualização de rendas não foram emitidas para o ano corrente",
                                Body = "Diploma é publicado em Outubro; cartas deverão ser enviadas antes do fim do ano.",
                                Link = "/arrendamentos",
                                ResourceType = "RentUpdate"
                            });
                        }
                    }
                    catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'rent letters issued' failed"); }
                }
            }

            // Leases ending within 4 months. Automatic renewal must NOT silence the alert:
            // the term still has to be reviewed/updated (1, 3 or more years).
            // Uses the same month-difference calculation as the Arrendamentos page.
            try
            {
                foreach (var lease in leases.Where(l => MonthsUntil(l.Data_Fim) <= 4))
                {
                    var autoRenew = settings?.RenovacaoAutomatica == true && lease.RenovacaoAutomatica;
                    var subject = $"{lease.NomeInquilino} ({lease.Fracao})";
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "LeaseEnding",
                        Severity = "Warning",
                        Title = autoRenew
                            ? $"Contrato do inquilino {subject} vai chegar ao fim — renovação prevista"
                            : $"Contrato do inquilino {subject} está prestes a terminar",
                        Body = autoRenew
                            ? "Renovação automática ativa; confirme o termo do contrato."
                            : "Renovar data-fim, ou enviar carta de revogação",
                        SuggestedAction = autoRenew
                            ? "Atualize a data-termo do contrato (1, 3 ou mais anos) ou confirme a renovação."
                            : "Renovar data-fim, ou enviar carta de revogação.",
                        Link = "/arrendamentos",
                        ResourceType = "Lease",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao,
                        AutoRenewal = autoRenew
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'lease ending' failed"); }

            // Rents due in the next 10 days. Mirrors CheckNewRents but only looks forward,
            // otherwise every historical lease would produce a permanent alert.
            try
            {
                var dueUntil = DateTime.Today.AddDays(10);
                foreach (var lease in leases.Where(l => l.Data_Pagamento.Date >= DateTime.Today &&
                                                        l.Data_Pagamento.Date <= dueUntil))
                {
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "RentDueSoon",
                        Severity = "Warning",
                        Title = $"Renda a vencer — {lease.NomeInquilino} ({lease.Fracao})",
                        Body = $"Data de pagamento: {lease.Data_Pagamento:dd/MM/yyyy}.",
                        SuggestedAction = "Confirmar a data de pagamento e preparar o movimento de renda.",
                        Link = "/recebimentos/manutencao",
                        ResourceType = "LeasePaymentDue",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'rent due soon' failed"); }

            // Overdue rents, grouped by tenant, matching the dashboard/tenant checks
            // (Estado == 3 or ValorEmFalta > 0).
            try
            {
                foreach (var group in payments.Where(p => p.Renda && (p.Estado == 3 || p.ValorEmFalta > 0))
                                              .GroupBy(p => p.ID_Inquilino))
                {
                    var name = group.Select(p => p.Inquilino)
                                    .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n)) ?? $"#{group.Key}";
                    var total = group.Sum(p => p.ValorEmFalta);
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "PaymentOverdue",
                        Severity = "Error",
                        Title = $"Rendas em atraso — {name}",
                        Body = $"{group.Count()} pagamento(s) em falta, num total de {total:C}.",
                        SuggestedAction = "Verificar os movimentos em atraso e enviar carta de aviso ao inquilino.",
                        Link = "/recebimentos/manutencao",
                        ResourceType = "TenantArrears",
                        ResourceId = group.Key,
                        PrimaryText = name
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'payment overdue' failed"); }

            // Missing late-rent warning letters: same rule used by IssueLateRentLetter
            // (more than 2 due months and fewer than dueMonths - 1 letters of type 18).
            try
            {
                foreach (var group in payments.Where(p => p.Renda && p.ValorEmFalta > 0)
                                              .GroupBy(p => p.ID_Inquilino))
                {
                    var dueMonths = group.Count();
                    if (dueMonths <= 2) continue;

                    var letters = tenantDocuments.Count(d => d.TenantId == group.Key && d.DocumentType == 18);
                    if (letters >= dueMonths - 1) continue;

                    var name = group.Select(p => p.Inquilino)
                                    .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n)) ?? $"#{group.Key}";
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "LateLetterMissing",
                        Severity = "Error",
                        Title = $"Faltam cartas de aviso de rendas em atraso — {name}",
                        Body = $"{dueMonths} meses em dívida e apenas {letters} carta(s) registada(s).",
                        SuggestedAction = "Emitir as cartas de aviso em falta antes de avançar com a revogação do contrato.",
                        Link = "/tenants",
                        ResourceType = "TenantLateLetters",
                        ResourceId = group.Key,
                        PrimaryText = name
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'late letter missing' failed"); }

            // Revocation letters without an answer inside the configured deadline
            // (the UI currently hardcodes 10 days when the setting is not usable).
            try
            {
                var responseDays = settings?.PrazoRespostaCartaRevogacao > 0
                    ? settings.PrazoRespostaCartaRevogacao
                    : (byte)10;
                foreach (var lease in leases.Where(l => l.EnvioCartaRevogacao &&
                                                        !l.RespostaCartaRevogacao &&
                                                        l.DataEnvioCartaRevogacao != default &&
                                                        l.DataEnvioCartaRevogacao.AddDays(responseDays).Date < DateTime.Today))
                {
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "LetterResponseOverdue",
                        Severity = "Error",
                        Title = $"Carta de revogação sem resposta — {lease.NomeInquilino} ({lease.Fracao})",
                        Body = $"Enviada em {lease.DataEnvioCartaRevogacao:dd/MM/yyyy}; prazo de {responseDays} dias ultrapassado.",
                        SuggestedAction = "Contactar o inquilino, registar a resposta ou avançar com a oposição à renovação.",
                        Link = "/arrendamentos",
                        ResourceType = "LeaseRevocationLetter",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'letter response overdue' failed"); }

            // Contract flagged as issued but without a generated document reference.
            try
            {
                foreach (var lease in leases.Where(l => l.ContratoEmitido && string.IsNullOrWhiteSpace(l.DocumentoGerado)))
                {
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "ContractDocumentMissing",
                        Severity = "Warning",
                        Title = $"Contrato emitido sem documento — {lease.NomeInquilino} ({lease.Fracao})",
                        Body = "O contrato está assinalado como emitido mas não tem ficheiro gerado registado.",
                        SuggestedAction = "Gerar novamente o contrato ou corrigir a indicação de emissão.",
                        Link = "/arrendamentos",
                        ResourceType = "LeaseContract",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao
                    });
                }
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Alert rule 'contract document missing' failed"); }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Operational alert calculation failed");
        }

        return alerts;
    }

    /// <summary>
    /// Same month-difference calculation used by the Arrendamentos page, so the shared rules
    /// produce exactly the same set of lease-ending warnings on both surfaces.
    /// </summary>
    private static int MonthsUntil(DateTime endDate)
    {
        var today = DateTime.Today;
        if (endDate.Date < today) return 0;
        return Math.Abs(12 * (today.Year - endDate.Year) + today.Month - endDate.Month);
    }

    /// <summary>
    /// Computes current alerts and inserts any that have not already been delivered today.
    /// Called when the bell/inbox loads so operational warnings appear in the personal inbox.
    /// </summary>
    public async Task<int> RefreshAlertsAsync(string recipientUserId)
    {
        if (string.IsNullOrWhiteSpace(recipientUserId)) return 0;

        var since = DateTime.UtcNow.AddDays(-1);
        var inserted = 0;
        try
        {
            foreach (var alert in await GetAlertsAsync())
            {
                try
                {
                    if (await _notifications.ExistsRecentAsync(recipientUserId, alert.Kind,
                            alert.ResourceType, alert.ResourceId, since))
                        continue;

                    var id = await NotifyAsync(new CreateNotificationVM
                    {
                        RecipientUserId = recipientUserId,
                        Kind = alert.Kind,
                        Title = alert.Title,
                        Body = alert.Body,
                        Link = alert.Link,
                        Severity = alert.Severity,
                        SuggestedAction = alert.SuggestedAction,
                        ResourceType = alert.ResourceType,
                        ResourceId = alert.ResourceId
                    });
                    if (id > 0) inserted++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to persist alert '{Kind}'", alert.Kind);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Alert refresh failed");
        }
        return inserted;
    }
}
