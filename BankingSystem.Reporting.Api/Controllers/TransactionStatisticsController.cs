using BankingSystem.Reporting.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Reporting.Api.Controllers;

[ApiController]
[Route("api/transaction-statistics")]
public class TransactionStatisticsController : ControllerBase
{
    private readonly ITransactionStatisticsService _transactionStatisticsService;

    public TransactionStatisticsController(ITransactionStatisticsService transactionStatisticsService)
    {
        _transactionStatisticsService = transactionStatisticsService;
    }

    [HttpGet("get-transactions-quantity")]
    public async Task<string> GetTransactionQuantityForLast1Month()
    {
        var transactionsOneMonth = await _transactionStatisticsService.GetTransactionQuantityForLastMonth(1);
        var transactionsSixMonth = await _transactionStatisticsService.GetTransactionQuantityForLastMonth(6);
        var transactionsOneYear = await _transactionStatisticsService.GetTransactionQuantityForLastYear(1);

        return $"Inner Transactions Last One Month: {transactionsOneMonth.Item1}\n" +
               $"Outside Transactions Last One Month: {transactionsOneMonth.Item2}\n" +
               $"Inner Transactions Last Six Month: {transactionsSixMonth.Item1}\n" +
               $"Outside Transactions Last Six Month: {transactionsSixMonth.Item2}\n" +
               $"Inner Transactions Last One Year: {transactionsOneYear.Item1}\n" +
               $"Outside Transactions Last One Year: {transactionsOneYear.Item2}\n";
    }

    [HttpGet("get-transaction-income")]
    public async Task<string> GetTransactionIncomeLast1Month()
    {
        var transactionsOneMonth = await _transactionStatisticsService.GetTransactionIncomeByMonth(1);
        var transactionsSixMonth = await _transactionStatisticsService.GetTransactionIncomeByMonth(6);
        var transactionsOneYear = await _transactionStatisticsService.GetTransactionIncomeByYear(1);

        return $"Inner Last One Month Transaction Income: {transactionsOneMonth.Item1}\n" +
               $"Outside Last One Month Transaction Income: {transactionsOneMonth.Item2}\n" +
               $"Total Last One Month Transaction Income: {transactionsOneMonth.Item3}\n" +
               $"Inner Last Six Month Transaction Income: {transactionsSixMonth.Item1}\n" +
               $"Outside Last Six Month Transaction Income: {transactionsSixMonth.Item2}\n" +
               $"Total Last Six Month Transaction Income: {transactionsSixMonth.Item3}\n" +
               $"Inner Last One Year Transaction Income: {transactionsOneYear.Item1}\n" +
               $"Outside Last One Year Transaction Income: {transactionsOneYear.Item2}\n" +
               $"Total Last One Year Transaction Income: {transactionsOneYear.Item3}\n";
    }
    
    [HttpGet("total-deposits-from-atms")]
    public async Task<string> TotalDepositFromAtm()
    {
        var depositFromAtm = await _transactionStatisticsService.TotalDepositFromAtm();

        return $"Total Deposit From Atms: {depositFromAtm}";
    }

    [HttpGet("transaction-quantity-by-days-for-one-month")]
    public async Task<Dictionary<int, long>?> QuantitiesByDaysForOneMonth()
    {
        var transactionsByDaysForLastMonth = await _transactionStatisticsService.QuantityOfTransactionsByDaysForLastMonth();

        return transactionsByDaysForLastMonth;
    }

    [HttpGet("avg-income-for-transaction-in-gel-usd-eur")]
    public async Task<string> GetAvgIncomeInGelUsdEur()
    {
        var avgIncome = await _transactionStatisticsService.GetAvgIncome();
        var text = $"Avg income in Gel: {avgIncome["Gel"]}\n" +
                   $"Avg income in Usd: {avgIncome["Usd"]}\n" +
                   $"Avg income in Eur: {avgIncome["Eur"]}\n";

        return text;
    }
}