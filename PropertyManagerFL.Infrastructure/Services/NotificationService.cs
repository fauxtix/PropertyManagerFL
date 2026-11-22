using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;
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
                    var docs = await _tenants.GetDocumentos();
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
                        var payments = await _payments.GetAll();
                        if (leases.Any() && payments is not null && payments.Any() &&
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

            // Leases ending within 4 months (only when renewals are manual).
            // Uses the same month-difference calculation as the original Arrendamentos page
            // so both surfaces produce exactly the same set of warnings.
            if (settings is not null && !settings.RenovacaoAutomatica)
            {
                foreach (var lease in leases.Where(l => MonthsUntil(l.Data_Fim) <= 4))
                {
                    alerts.Add(new NotificationAlertVM
                    {
                        Kind = "LeaseEnding",
                        Severity = "Warning",
                        Title = $"Contrato do inquilino {lease.NomeInquilino} ({lease.Fracao}) está prestes a terminar",
                        Body = "Renovar data-fim, ou enviar carta de revogação",
                        Link = "/arrendamentos",
                        ResourceType = "Lease",
                        ResourceId = lease.Id,
                        PrimaryText = lease.NomeInquilino,
                        SecondaryText = lease.Fracao
                    });
                }
            }
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
