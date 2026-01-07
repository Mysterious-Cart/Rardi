using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using CHKS.Data;
using CHKS.Models.mydb;
using Microsoft.EntityFrameworkCore;


namespace CHKS.Pages
{
    public partial class Histories
    {

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public mydbService MydbService { get; set; }

        [Inject]
        public RardiContext RardiContext { get; set; }

        private IQueryable< History> histories;

        private RadzenDataGrid<History> grid0;

        private bool editMode = false;
        private string date;

        private string search = "";

        private string ChosenDate;

        [Inject]
        protected SecurityService Security { get; set; }

        private async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            await GetCustomerRecord();
        }

        protected override async Task OnInitializedAsync()
        {
           await GetCustomerRecord();
        }

        private async Task GetCustomerRecord()
        {
            histories = RardiContext.Histories.Include(i => i.Historyconnectors).Where(i => i.IsDeleted == 0 && (i.CashoutDate.Contains(search) || i.Plate.Contains(search)));
        }

        private async Task OpenHistory(History args){
            if(editMode == false){
                await DialogService.OpenAsync<ReciptView>("", new Dictionary<string, object>{{"ID",args.CashoutDate}}, new DialogOptions{Width="50%", Height="70%"});
            }
        }   



        private async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await MydbService.ExportHistoriesToCSV(new Query{
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Car",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Histories");
            }

            if (args == null || args.Value == "xlsx")
            {
                await MydbService.ExportHistoriesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Car",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Histories");
            }
        }

        private async Task GridDeleteButtonClick( CHKS.Models.mydb.History history)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure?") == true)
                {
                    history.IsDeleted = 1;
                    history.Info = "Deleted By:" + Security.User?.Name + "("+ DateTime.Now +")";
                    await MydbService.UpdateHistory(history.CashoutDate, history);
                    await grid0.Reload();
                }else{
                    editMode = false;
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete History"
                    
                });
            }
        }

        private async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            ChosenDate = data.CashoutDate;
            await grid0.EditRow(data);
            editMode = true;
        }

        private async Task GridRowUpdate(CHKS.Models.mydb.History args)
        {
                
            await MydbService.UpdateHistory(args.CashoutDate,args);
            editMode = false;

        }


        private async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            Console.WriteLine(ChosenDate);
            if(ChosenDate == "01/01/0001" ){
                await DialogService.Alert("No New Date given.","Important");
                await CancelButtonClick(args,data);
            }else{
                data.CashoutDate = ChosenDate;
                await grid0.UpdateRow(data);
            }
        }

        private async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            grid0.CancelEditRow(data);
            await MydbService.CancelHistoryChanges(data);
            editMode = false;
        }
    }
}