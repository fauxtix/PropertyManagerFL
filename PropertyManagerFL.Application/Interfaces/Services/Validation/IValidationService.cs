using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Contactos;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.Documentos;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Interfaces.Services.Validation
{
    public interface IValidationService
    {
        List<string> ValidatePropertyEntries(ImovelVM PropertyToValidate);
        List<string> ValidateUnitEntries(FracaoVM UnitToValidate);
        List<string> ValidateTenantEntries(InquilinoVMEx SelectedTenant);
        List<string> ValidateFiadorEntries(FiadorVM SelectedFiador);
        List<string> ValidateTableEntries<T>(T model);
        List<string> ValidateContactsEntries(ContactoVM contactToValidate);
        List<string> ValidateLeasesEntries(ArrendamentoVM leaseToValidate);
        List<string> ValidatePaymentEntries(DespesaVM paymentToValidate);
        List<string> ValidateExpenseTypeEntries(TipoDespesaVM expenseTypeToValidate);
        List<string> ValidateTransactonsEntries(RecebimentoVM transactionToValidate);
        List<string> ValidateDocumentEntries(DocumentoVM documentToValidate);
        List<string> ValidateLandlordEntry(ProprietarioVM landlordToValidate);
    }
}