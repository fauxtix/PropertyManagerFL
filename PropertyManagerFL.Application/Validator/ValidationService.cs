using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
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
using PropertyManagerFL.Application.ViewModels.Appointments;

namespace PropertyManagerFL.Application.Validator
{
    public class ValidationService : IValidationService
    {
        private readonly InquilinoValidator _tenantsValidator;
        private readonly FiadorValidator _fiadoresValidator;
        private readonly ImovelValidator _propertiesValidator;
        private readonly FracaoValidator _unitsValidator;
        private readonly ContactoValidator _contactsValidator;
        private readonly ArrendamentoValidator _leasesValidator;
        private readonly DespesaValidator _paymentsValidator;
        private readonly DocumentoValidator _documentsValidator;
        private readonly TipoDespesaValidator _expenseTypeValidator;
        private readonly RecebimentoValidator _transactionsValidator;
        private readonly ApointmentValidator _apptsValidator;
        private readonly ProprietarioValidator _landlordService;

        public ValidationService(InquilinoValidator tenantsValidator,
                                 ImovelValidator propertiesValidator,
                                 FracaoValidator unitsValidator,
                                 ContactoValidator contactsValidator,
                                 ArrendamentoValidator leasesValidator,
                                 DespesaValidator paymentsValidator,
                                 TipoDespesaValidator expenseTypeValidator,
                                 RecebimentoValidator transactionsValidator,
                                 DocumentoValidator documentsValidator,
                                 FiadorValidator fiadoresValidator,
                                 ProprietarioValidator proprietarioService,
                                 ProprietarioValidator landlordService,
                                 ApointmentValidator apptsValidator)
        {
            _tenantsValidator = tenantsValidator;
            _propertiesValidator = propertiesValidator;
            _unitsValidator = unitsValidator;
            _contactsValidator = contactsValidator;
            _leasesValidator = leasesValidator;
            _paymentsValidator = paymentsValidator;
            _expenseTypeValidator = expenseTypeValidator;
            _transactionsValidator = transactionsValidator;
            _documentsValidator = documentsValidator;
            _fiadoresValidator = fiadoresValidator;
            _landlordService = landlordService;
            _apptsValidator = apptsValidator;
        }
        public List<string> ValidateTenantEntries(InquilinoVMEx SelectedTenant)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _tenantsValidator.Validate(SelectedTenant);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }
        public List<string> ValidatePropertyEntries(ImovelVM SelectedProperty)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _propertiesValidator.Validate(SelectedProperty);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }

        public List<string> ValidateUnitEntries(FracaoVM UnitToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _unitsValidator.Validate(UnitToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }

        public List<string> ValidateContactsEntries(ContactoVM contactToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _contactsValidator.Validate(contactToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }


        public List<string> ValidateTableEntries<T>(T model)
        {
            List<string> sValidationErrors = new List<string>();
            var validator = new TabAuxGenericValidator<T>();
            ValidationResult results = validator.Validate(model);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }

        public List<string> ValidateLeasesEntries(ArrendamentoVM leaseToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _leasesValidator.Validate(leaseToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;

        }

        public List<string> ValidatePaymentEntries(DespesaVM paymentToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _paymentsValidator.Validate(paymentToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;

        }
        public List<string> ValidateExpenseTypeEntries(TipoDespesaVM expenseTypeToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _expenseTypeValidator.Validate(expenseTypeToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;

        }

        public List<string> ValidateDocumentEntries(DocumentoVM documentToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _documentsValidator.Validate(documentToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;

        }


        public List<string> ValidateTransactonsEntries(RecebimentoVM transactionToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _transactionsValidator.Validate(transactionToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }

        public List<string> ValidateFiadorEntries(FiadorVM selectedFiador)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _fiadoresValidator.Validate(selectedFiador);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }


        public List<string> ValidateAppointmentEntries(AppointmentVM selectedAppointment)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _apptsValidator.Validate(selectedAppointment);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }

        public List<string> ValidateLandlordEntry(ProprietarioVM landlordToValidate)
        {
            List<string> sValidationErrors = new List<string>();

            ValidationResult results = _landlordService.Validate(landlordToValidate);
            if (!results.IsValid)
            {
                foreach (ValidationFailure failure in results.Errors)
                {
                    sValidationErrors.Add(failure.ErrorMessage);
                }
                return sValidationErrors;
            }
            else
                return null;
        }
    }
}
