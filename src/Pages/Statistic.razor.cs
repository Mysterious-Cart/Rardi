using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Globalization;
using CHKS.Models.mydb;
using CHKS.Services;

namespace CHKS.Pages
{
    public partial class Statistic
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected mydbService MydbService {get; set;}

        [Inject]
        protected RardiReportService RardiReportService {get; set;}

        private TransactionReport? ReportData;
        private IReadOnlyList<TransactionPerformancePoint> PerformancePoints = Array.Empty<TransactionPerformancePoint>();
        private IReadOnlyList<History> RecentTransactions = Array.Empty<History>();
        private DateTime TimeEnd = DateTime.Today;
        private DateTime? TimeStart = DateTime.Today.AddDays(-29);

        protected override async Task OnInitializedAsync()
        {
            await RefreshPage();
        }

        private async Task RefreshPage()
        {
            if (TimeStart.HasValue && TimeStart > TimeEnd)
            {
                TimeStart = TimeEnd.AddDays(-29);
            }

            await LoadData();
            StateHasChanged();
        }

        private async Task LoadData()
        {
            ReportData = await RardiReportService.GetReport(TimeStart, TimeEnd);
            PerformancePoints = await RardiReportService.GetPerformanceTimeline(TimeStart, TimeEnd);
            RecentTransactions = await RardiReportService.GetTransactions(TimeStart, TimeEnd);
        }

        private async Task ApplyQuickRangeAsync(int days)
        {
            TimeEnd = DateTime.Today;
            TimeStart = DateTime.Today.AddDays(-(days - 1));
            await RefreshPage();
        }

        private static string FormatCurrency(decimal? value) => string.Format(CultureInfo.InvariantCulture, "${0:N2}", value ?? 0);

        private decimal RevenueTotal => Math.Round((ReportData?.Category.Product ?? 0) + (ReportData?.Category.Service ?? 0), 2);
        private decimal CogsTotal => Math.Round(ReportData?.Category.Import ?? 0, 2);
        private decimal NetProfit => Math.Round(ReportData?.NetProfit ?? 0, 2);
        private decimal GrossMarginPercent => RevenueTotal == 0 ? 0 : Math.Round(((RevenueTotal - CogsTotal) / RevenueTotal) * 100, 2);

        private IEnumerable<PaymentSlice> PaymentSlices => new List<PaymentSlice>
        {
            new("Bank", ReportData?.Income.Bank ?? 0),
            new("Dollar", ReportData?.Income.Dollar ?? 0),
            new("Baht", ReportData?.Income.Baht ?? 0),
            new("Riel", ReportData?.Income.Riel ?? 0)
        }.Where(s => s.Value > 0).DefaultIfEmpty(new PaymentSlice("No payments", 1));

        private string PaymentMethodLabel(History history)
        {
            var methods = new List<string>();

            if ((history.Bank ?? 0) > 0) methods.Add("Bank");
            if ((history.Dollar ?? 0) > 0) methods.Add("Cash (USD)");
            if ((history.Baht ?? 0) > 0) methods.Add("Cash (THB)");
            if ((history.Riel ?? 0) > 0) methods.Add("Cash (KHR)");

            return methods.Count == 0 ? "N/A" : string.Join(" / ", methods);
        }

        private async Task OpenHistory(History args)
        {
            await DialogService.OpenAsync<ReciptView>("", new Dictionary<string, object> { { "ID", args.CashoutDate } }, new DialogOptions { Width = "50%", Height = "70%" });
            await RefreshPage();
        }

    }

    public record PaymentSlice(string Label, decimal Value);
}