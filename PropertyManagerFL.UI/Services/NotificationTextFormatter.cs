using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.ViewModels.Notifications;

namespace PropertyManagerFL.UI.Services;

/// <summary>
/// Turns the canonical (Portuguese) notification text produced by the API rules engine into
/// localized text for the current UI culture. Keeping the mapping here means the API, the
/// database and the stored notifications do not need to change when a culture is added.
/// </summary>
public interface INotificationTextFormatter
{
    string Title(NotificationVM item);
    string Body(NotificationVM item);
    string SuggestedAction(NotificationVM item);
    string SuggestedAction(NotificationAlertVM alert);
    string RelativeTime(DateTime createdAt);
    string GroupFor(DateTime createdAt);
}

public class NotificationTextFormatter : INotificationTextFormatter
{
    private sealed record TextRule(string Portuguese, string ResourceKey);

    private sealed record KindRules(
        string Kind,
        TextRule[] Titles,
        TextRule[] Bodies,
        TextRule[] Suggestions);

    private static readonly KindRules[] Rules =
    [
        new("RentLetter",
            [new TextRule("Necessário envio de carta de atualização", "NotificationRentLetterTitle")],
            [],
            []),
        new("RentUpdate",
            [new TextRule("Necessário atualizar renda do inquilino {0} ({1})", "Necessário atualizar renda do inquilino {0} ({1})")],
            [new TextRule("não foi enviada carta de atualização!", "NotificationRentUpdateMissingLetter")],
            []),
        new("RentLetters",
            [new TextRule("Cartas de atualização de rendas não foram emitidas para o ano corrente", "Cartas de atualização de rendas não foram emitidas para o ano corrente")],
            [new TextRule("Diploma é publicado em Outubro; cartas deverão ser enviadas antes do fim do ano.", "NotificationRentLettersBody")],
            []),
        new("LeaseEnding",
            [
                new TextRule("Contrato do inquilino {0} ({1}) vai chegar ao fim — renovação prevista", "LeaseEndingAutoRenewal"),
                new TextRule("Contrato do inquilino {0} ({1}) está prestes a terminar", "NotificationLeaseEndingTitle")
            ],
            [
                new TextRule("Renovação automática ativa; confirme o termo do contrato.", "NotificationLeaseEndingAutoRenewalBody"),
                new TextRule("Renovar data-fim, ou enviar carta de revogação", "NotificationLeaseEndingBody")
            ],
            [
                new TextRule("Atualize a data-termo do contrato (1, 3 ou mais anos) ou confirme a renovação.", "NotificationLeaseEndingAutoRenewalSuggested"),
                new TextRule("Renovar data-fim, ou enviar carta de revogação.", "NotificationLeaseEndingSuggested")
            ]),
        new("RentDueSoon",
            [new TextRule("Renda a vencer — {0} ({1})", "NotificationRentDueSoonTitle")],
            [new TextRule("Data de pagamento: {0}.", "NotificationRentDueSoonBody")],
            [new TextRule("Confirmar a data de pagamento e preparar o movimento de renda.", "NotificationRentDueSoonSuggested")]),
        new("PaymentOverdue",
            [new TextRule("Rendas em atraso — {0}", "NotificationPaymentOverdueTitle")],
            [new TextRule("{0} pagamento(s) em falta, num total de {1}.", "NotificationPaymentOverdueBody")],
            [new TextRule("Verificar os movimentos em atraso e enviar carta de aviso ao inquilino.", "NotificationPaymentOverdueSuggested")]),
        new("LateLetterMissing",
            [new TextRule("Faltam cartas de aviso de rendas em atraso — {0}", "NotificationLateLetterMissingTitle")],
            [new TextRule("{0} meses em dívida e apenas {1} carta(s) registada(s).", "NotificationLateLetterMissingBody")],
            [new TextRule("Emitir as cartas de aviso em falta antes de avançar com a revogação do contrato.", "NotificationLateLetterMissingSuggested")]),
        new("LetterResponseOverdue",
            [new TextRule("Carta de revogação sem resposta — {0} ({1})", "NotificationLetterResponseOverdueTitle")],
            [new TextRule("Enviada em {0}; prazo de {1} dias ultrapassado.", "NotificationLetterResponseOverdueBody")],
            [new TextRule("Contactar o inquilino, registar a resposta ou avançar com a oposição à renovação.", "NotificationLetterResponseOverdueSuggested")]),
        new("ContractDocumentMissing",
            [new TextRule("Contrato emitido sem documento — {0} ({1})", "NotificationContractDocumentMissingTitle")],
            [new TextRule("O contrato está assinalado como emitido mas não tem ficheiro gerado registado.", "NotificationContractDocumentMissingBody")],
            [new TextRule("Gerar novamente o contrato ou corrigir a indicação de emissão.", "NotificationContractDocumentMissingSuggested")])
    ];

    private readonly IStringLocalizer<App> _localizer;

    public NotificationTextFormatter(IStringLocalizer<App> localizer)
    {
        _localizer = localizer;
    }

    public string Title(NotificationVM item) => Format(item.Kind, item.Title, r => r.Titles);

    public string Body(NotificationVM item) => Format(item.Kind, item.Body, r => r.Bodies);

    public string SuggestedAction(NotificationVM item)
        => Format(item.Kind, item.SuggestedAction, r => r.Suggestions);

    public string SuggestedAction(NotificationAlertVM alert)
        => Format(alert.Kind, alert.SuggestedAction, r => r.Suggestions);

    public string RelativeTime(DateTime createdAt)
    {
        var local = createdAt.ToLocalTime();
        var delta = DateTime.Now - local;
        if (delta.TotalMinutes < 1) return _localizer["TituloAgora"].Value;
        if (delta.TotalMinutes < 60) return _localizer["NotificationsMinutesShort", (int)delta.TotalMinutes].Value;
        if (delta.TotalHours < 24) return _localizer["NotificationsHoursShort", (int)delta.TotalHours].Value;
        if (delta.TotalDays < 7) return _localizer["NotificationsDaysShort", (int)delta.TotalDays].Value;
        return local.ToString("d", CultureInfo.CurrentCulture);
    }

    public string GroupFor(DateTime createdAt)
    {
        var local = createdAt.ToLocalTime().Date;
        var today = DateTime.Today;
        if (local == today) return _localizer["NotificationsToday"].Value;
        if (local == today.AddDays(-1)) return _localizer["NotificationsYesterday"].Value;
        return local.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
    }

    private string Format(string kind, string? stored, Func<KindRules, TextRule[]> selector)
    {
        if (string.IsNullOrWhiteSpace(stored)) return string.Empty;

        var rules = Rules.FirstOrDefault(r => r.Kind == kind);
        if (rules is null) return stored;

        foreach (var rule in selector(rules))
        {
            if (!rule.Portuguese.Contains("{0}"))
            {
                if (string.Equals(stored.Trim(), rule.Portuguese, StringComparison.OrdinalIgnoreCase))
                    return _localizer[rule.ResourceKey].Value;
                continue;
            }

            var regex = BuildRegex(rule.Portuguese);
            var match = regex.Match(stored.Trim());
            if (!match.Success) continue;

            var args = rule.Portuguese.Contains("{0}")
                ? match.Groups.Values.Skip(1).Select(g => (object)g.Value).ToArray()
                : Array.Empty<object>();
            return _localizer[rule.ResourceKey, args].Value;
        }

        return stored;
    }

    private static Regex BuildRegex(string template)
    {
        var escaped = Regex.Escape(template);
        var pattern = Regex.Replace(escaped, @"\\\{(\d+)\\\}", m => $"(?<a{m.Groups[1].Value}>.*?)");
        return new Regex($"^{pattern}$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
    }
}
