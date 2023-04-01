using BankingSystem.Reporting.Core.Repositories;
using BankingSystem.Reporting.Core.Validations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BankingSystem.Reporting.Api.Services;

public interface ITransactionStatisticsService
{
    Task<JObject> GetAvgIncome();
    Task<(int, int)> GetTransactionQuantityForLastMonth(int month);
    Task<(int, int)> GetTransactionQuantityForLastYear(int year);
    Task<(decimal, decimal, decimal)> GetTransactionIncomeByMonth(int month);
    Task<string> PrintQuantityOfTransactionsByDaysForLastMonth();
    Task<(decimal, decimal, decimal)> GetTransactionIncomeByYear(int year);
    Task<decimal> TotalDepositFromAtm();
    Task<Dictionary<int, long>?> QuantityOfTransactionsByDaysForLastMonth();
}

public class TransactionStatisticsService : ITransactionStatisticsService
{
    private readonly ITransactionStatisticsValidations _transactionStatisticsValidations;
    private readonly ITransactionStatisticsRepository _transactionStatisticsRepository;

    public TransactionStatisticsService(
        ITransactionStatisticsValidations transactionStatisticsValidations, 
        ITransactionStatisticsRepository transactionStatisticsRepository)
    {
        _transactionStatisticsValidations = transactionStatisticsValidations;
        _transactionStatisticsRepository = transactionStatisticsRepository;
    }

    public async Task<JObject> GetAvgIncome()
    {
        var avgIncome = await _transactionStatisticsValidations.AvgIncomeFromTransactionGelUsdEur();
        var gel = avgIncome.Item1;
        var usd = avgIncome.Item2;
        var eur = avgIncome.Item3;

        var json = new JObject();
        json.Add("Gel", gel);
        json.Add("Usd", usd);
        json.Add("Eur", eur);

        if (json == null)
        {
            throw new Exception("could not find avg incomes");
        }

        return json;
    }

    public async Task<(int, int)> GetTransactionQuantityForLastMonth(int month)
    {
        var innerTransactionsLastMonths = await _transactionStatisticsRepository.TransactionsLastMonths(month).Item1.ToListAsync();
        var outsideTransactionsLastMonths = await _transactionStatisticsRepository.TransactionsLastMonths(month).Item2.ToListAsync();

        var innerQuantity = innerTransactionsLastMonths.Count;
        var outsideQuantity = outsideTransactionsLastMonths.Count;
        
        return (innerQuantity, outsideQuantity);
    }
    
    public async Task<(int, int)> GetTransactionQuantityForLastYear(int year)
    {
        var innerTransactionsLastMonths = await _transactionStatisticsRepository.TotalTransactionsLastYear(year).Item1.ToListAsync();
        var outsideTransactionsLastMonths = await _transactionStatisticsRepository.TotalTransactionsLastYear(year).Item2.ToListAsync();

        var innerQuantity = innerTransactionsLastMonths.Count;
        var outsideQuantity = outsideTransactionsLastMonths.Count;
        
        return (innerQuantity, outsideQuantity);
    }

    public async Task<(decimal, decimal, decimal)> GetTransactionIncomeByMonth(int month)
    {
        var income = await _transactionStatisticsValidations.TotalIncomeFromTransactionsByMonth(month);

        return income;
    }
    
    public async Task<(decimal, decimal, decimal)> GetTransactionIncomeByYear(int year)
    {
        var income = await _transactionStatisticsValidations.TotalIncomeFromTransactionsByYear(year);

        return income;
    }

    public async Task<decimal> TotalDepositFromAtm()
    {
        var totalDeposit = await _transactionStatisticsValidations.TotalAtmDeposit();

        return totalDeposit;
    }

    public async Task<Dictionary<int, long>?> QuantityOfTransactionsByDaysForLastMonth()
    {
        var transactions = await _transactionStatisticsRepository.TotalQuantitiesBasedOnDays();
        
        return transactions;
    }

    public async Task<string> PrintQuantityOfTransactionsByDaysForLastMonth()
    {
        var transactions = await QuantityOfTransactionsByDaysForLastMonth();
        var text = "";
        if (transactions == null)
        {
            throw new Exception("Transactions doesn't exist");
        }

        foreach (var transaction in transactions)
        {
            text += $"Day {transaction.Key}: {transaction.Value}\n";
        }
        
        return text;
    }
}