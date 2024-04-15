using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Appointments;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace PropertyManagerFL.Infrastructure.Adapters
{
    public class AppointmentAdaptor : DataAdaptor
    {
        private readonly IAppointmentsService _apptService;
        public AppointmentAdaptor(IAppointmentsService appService)
        {
            _apptService = appService;
        }

        IEnumerable<AppointmentVM>? EventData; 
        public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            EventData = await GetAppts();
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = EventData, Count = EventData.Count() } : EventData;
        }
        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            var apptId = await _apptService.InsertAsync((AppointmentVM)data);
            return data;
        }
        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            var appt = (AppointmentVM)data;
            var apptId = appt.Id;
            await _apptService.UpdateAsync(apptId, appt);
            return data;
        }
        public async override Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key) //triggers on appointment deletion through public method DeleteEvent
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            var apptID = (int)data;
            await _apptService.DeleteAsync(apptID);
            return data;
        }
        public async override Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords, object addedRecords, object deletedRecords, string keyField, string key, int? dropIndex)
        {
            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            object records = deletedRecords;
            List<AppointmentVM>? deleteData = (List<AppointmentVM>)deletedRecords;
            if (deleteData is not null && deleteData.Count > 0)
            {
                foreach (var data in deleteData)
                {
                    await _apptService.DeleteAsync(data.Id);
                }
            }
            List<AppointmentVM>? addData = (List<AppointmentVM>)addedRecords;
            if (addData is not null && addData.Count > 0)
            {
                foreach (var data in addData)
                {
                    await _apptService.InsertAsync(data);
                    records = addedRecords;
                }
            }

            List<AppointmentVM>? updateData = (List<AppointmentVM>?)changedRecords;
            if (updateData is not null && updateData.Count > 0)
            {
                foreach (AppointmentVM? data in updateData)
                {
                    var apptId = data.Id;

                    await _apptService.UpdateAsync(apptId, data);
                    records = changedRecords;
                }
            }
            return records;
        }

        private async Task<List<AppointmentVM>> GetAppts()
        {
            return (await _apptService.GetAllAsync()).ToList();
        }
    }
}

