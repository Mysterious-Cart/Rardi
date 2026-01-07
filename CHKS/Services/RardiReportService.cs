using CHKS.Data;
using CHKS.Models.mydb;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CHKS.Services;

public class RardiReportService 
{
    private readonly RardiContext _rardiContext;
    

    public RardiReportService(RardiContext rardiContext)
    {
        _rardiContext = rardiContext;
    }

    private IQueryable<History> Transactions;
    private IQueryable<Historyconnector> TransactionItems;

    private async Task<TotalTransactionCategory> GetTransactionCategory(DateTime startDate, DateTime endDate)
    {

        decimal ServiceTotal = 0;
        decimal ProductTotal = 0;
        decimal ImportTotal = 0;
        decimal CompanyTotal = 0;

        var LocalTransactionItems = _rardiContext.Historyconnectors
            .Include(i => i.Inventory)
            .Where(i => i.History.PaidAt >= startDate && i.History.PaidAt <= endDate.AddDays(1))
            .Where(i => i.History.Company == 0);

        List<Historyconnector> ServiceCharge = await LocalTransactionItems.Where(i => i.Product == "Service Charge").ToListAsync();
        List<Historyconnector> Products = await LocalTransactionItems.Where(i => i.Product != "Service Charge").ToListAsync();

        foreach (var TransactionItem in ServiceCharge)
        {   
            ServiceTotal += TransactionItem.Export ?? 0; // Service charge quantity is always 1
            var importCost = (TransactionItem.Inventory?.Import ?? 0) * (TransactionItem.Qty ?? 1);
            ImportTotal += importCost;
        }

        foreach (var TransactionItem in Products)
        {
            ProductTotal += (TransactionItem.Export ?? 0) * (TransactionItem.Qty ?? 1);
            var importCost = (TransactionItem.Inventory?.Import ?? 0) * (TransactionItem.Qty ?? 1);
            ImportTotal += importCost;
        }
        return new TotalTransactionCategory(ServiceTotal, ProductTotal, ImportTotal, CompanyTotal);
    }

    private async Task<TotalTransactionPaymentType> GetTotalTransactionIncomeType()
    {
        var Temp = Transactions.Where(i => i.Company == 0).Select(i => new { i.Baht, i.Dollar, i.Riel, i.Bank }).ToList();
        var TotalBaht = Temp.Sum(i => i.Baht ?? 0);
        var TotalDollar = Temp.Sum(i => i.Dollar ?? 0);
        var TotalRiel = Temp.Sum(i => i.Riel ?? 0);
        var TotalBank = Temp.Sum(i => i.Bank ?? 0);
        return new TotalTransactionPaymentType(TotalDollar, TotalBaht, TotalRiel, TotalBank);

    }

    private async Task<List<Historyconnector>> GetTransactionItemsWithoutImport(DateTime startDate, DateTime endDate)
    {        
        var ProductWithoutImport = _rardiContext.Historyconnectors
            .Include(i => i.Inventory)
            .Where(i => i.History.PaidAt >= startDate && i.History.PaidAt <= endDate.AddDays(1))
            .Where(i => i.Product != "Service Charge")
            .Where(i => i.Inventory.Import == 0 || i.Inventory.Import == null).ToList();
        return ProductWithoutImport;
    }

    private async Task<decimal> GetExpenses(DateTime? TimeStart, DateTime TimeEnd)
    {
        
        return _rardiContext.Dailyexpenses.Where(i => i.CreatedAt > TimeStart && i.CreatedAt <= TimeEnd).Sum(i => i.Expense) ?? 0;
    }
    private async Task<decimal> CalcNetProfit(TotalTransactionCategory totals, decimal TotalExpense)
    {
        return Math.Round((totals.Product - totals.Import) + totals.Service - TotalExpense, 2);
    }

    public async Task<List<TransactionPerformancePoint>> GetPerformanceTimeline(DateTime? startDate, DateTime endDate)
    {
        var start = (startDate ?? DateTime.MinValue).Date;
        var end = endDate.Date.AddDays(1);

        var revenueByDate = (await _rardiContext.Histories
            .Where(h => h.Company == 0)
            .Where(h => h.PaidAt >= start && h.PaidAt < end)
            .Where(h => h.PaidAt != null)
            .Select(h => new { Date = h.PaidAt.Value.Date, Total = h.Total ?? 0 })
            .ToListAsync())
            .GroupBy(i => i.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Total));

        var cogsByDate = (await _rardiContext.Historyconnectors
            .Include(c => c.Inventory)
            .Include(c => c.History)
            .Where(c => c.History.Company == 0)
            .Where(c => c.History.PaidAt >= start && c.History.PaidAt < end)
            .Where(c => c.History.PaidAt != null)
            .Select(c => new { Date = c.History.PaidAt.Value.Date, Cogs = (c.Inventory.Import ?? 0) * (c.Qty ?? 1) })
            .ToListAsync())
            .GroupBy(i => i.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Cogs));

        var expensesByDate = (await _rardiContext.Dailyexpenses
            .Where(e => e.CreatedAt >= start && e.CreatedAt < end)
            .Select(e => new { Date = e.CreatedAt!.Value.Date, Expense = e.Expense ?? 0 })
            .ToListAsync())
            .GroupBy(i => i.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Expense));

        var allDates = revenueByDate.Keys
            .Concat(cogsByDate.Keys)
            .Concat(expensesByDate.Keys)
            .Distinct()
            .OrderBy(d => d);

        var points = new List<TransactionPerformancePoint>();

        foreach (var date in allDates)
        {
            revenueByDate.TryGetValue(date, out var revenue);
            cogsByDate.TryGetValue(date, out var cogs);
            expensesByDate.TryGetValue(date, out var expense);

            var profit = revenue - cogs - expense;
            var grossMarginPercent = revenue == 0 ? 0 : Math.Round(((revenue - cogs) / revenue) * 100, 2);

            points.Add(new TransactionPerformancePoint(date, revenue, cogs, profit, grossMarginPercent));
        }

        return points;
    }

    public async Task<List<History>> GetTransactions(DateTime? startDate, DateTime endDate, int take = 250)
    {
        var start = (startDate ?? DateTime.MinValue).Date;
        var end = endDate.Date.AddDays(1);

        return await _rardiContext.Histories
            .Include(h => h.Car)
            .Where(h => h.Company == 0)
            .Where(h => h.PaidAt >= start && h.PaidAt < end)
            .OrderByDescending(h => h.PaidAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<TransactionReport> GetReport(DateTime? StartDate, DateTime EndDate)
    {
        if (StartDate == null)
        {
            StartDate = DateTime.MinValue;
        }
        Transactions = _rardiContext.Histories
            .Where(i => i.PaidAt >= StartDate && i.PaidAt <= EndDate.AddDays(1));
        var transactionReportCategory = await GetTransactionCategory(StartDate.Value, EndDate);
        var transactionReportIncomeType =  await GetTotalTransactionIncomeType();
        var transactionItemsWithoutImport = await GetTransactionItemsWithoutImport(StartDate.Value, EndDate);
        var expense = await GetExpenses(StartDate, EndDate);
        var netProfit = await CalcNetProfit(transactionReportCategory, expense);
        return new TransactionReport(transactionReportCategory, transactionReportIncomeType, transactionItemsWithoutImport, expense, netProfit);
    }
    // Add methods for generating reports as needed
}

public record TransactionReport(TotalTransactionCategory Category, TotalTransactionPaymentType Income, List<Historyconnector> TransactionItemsMissingImport, decimal Expense, decimal NetProfit);
public record TotalTransactionPaymentType(decimal Dollar, decimal Baht, decimal Riel, decimal Bank);
public record TotalTransactionCategory(decimal Service, decimal Product, decimal Import, decimal Company);
public record TransactionPerformancePoint(DateTime Date, decimal Revenue, decimal Cogs, decimal Profit, decimal GrossMarginPercent);