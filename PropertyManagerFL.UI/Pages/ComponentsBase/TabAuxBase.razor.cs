using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using System.Collections;
using System.Collections.ObjectModel;
using System.Dynamic;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class TabAuxBase : ComponentBase, IDisposable
    {
        protected int RecordId;
        protected int ToastTimeOut = 3000;

        [Inject] protected ILookupTableService? auxTablesService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }
        [Inject] protected IStringLocalizer<App> L { get; set; }



        // Dialogs visibility flags
        protected bool EditDialogVisibility { get; set; } = false;
        protected bool ValidationErrorsVisibility { get; set; } = false;
        protected bool DeleteConfirmVisibility { get; set; } = false;
        protected bool ErrorVisibility { get; set; } = false;

        protected string ToastTitle = "";
        protected string ToastContent = "";
        protected string ToastCssClass = "";

        protected bool editRecord = true;
        protected List<string?>? lstErrorMsg;  // validation message(s)

        protected SfToast? ToastObj { get; set; }
        protected DialogEffect efeitos = DialogEffect.Zoom;


        protected int Id { get; set; }
        protected string? Description { get; set; }


        public DialogEffect Effects = DialogEffect.Zoom;

        protected List<ExpandoObject> GenericModelList { get; set; } = new List<ExpandoObject>();

        /// <summary>
        /// Get Data and convert to list of ExpandoObjects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceDbTable"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExpandoObject>> GetDataGenerics<T>(string sourceDbTable) where T : class
        {
            var GenericList = (await auxTablesService.GetLookupTableData(sourceDbTable)).ToList().OrderBy(o => o.Descricao);
            foreach (var item in GenericList)
            {
                dynamic GenericModel = new ExpandoObject();
                GenericModel.Codigo = item.Id;
                GenericModel.Descricao = item.Descricao;
                GenericModelList.Add(GenericModel);
            }

            IEnumerable<ExpandoObject> outputList = GenericModelList.Cast<ExpandoObject>().ToList();
            GenericModelList.Clear(); // = new List<ExpandoObject>(); // se não incluir esta linha, os dados aparecem sempre a dobrar, em cada Insert/Delete
            return outputList;
        }


        protected async Task<bool> RecordExist(string description, string table)
        {
            return await auxTablesService.CheckIfRecordExist(description, table);
        }

        /// <summary>
        /// Not Used
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public ObservableCollection<T> Convert<T>(IEnumerable original)
        {
            return new ObservableCollection<T>(original.Cast<T>());
        }

        public void closeAlertBox()
        {
            ValidationErrorsVisibility = false;
            DeleteConfirmVisibility = false;
        }

        public void closeErrorBox()
        {
            ErrorVisibility = false;
            EditDialogVisibility = true;
        }

        public void ConfirmDeleteNo()
        {
            DeleteConfirmVisibility = false;
        }

        public void CloseEditDialog()
        {
            editRecord = true;

            EditDialogVisibility = false;
        }

        public void Dispose()
        {
            ToastObj?.Dispose();
        }
    }
}
