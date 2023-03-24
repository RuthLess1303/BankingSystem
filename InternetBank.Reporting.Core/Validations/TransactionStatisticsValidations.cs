using InternetBank.Db.Db.Repositories;
using InternetBank.Reporting.Core.Repositories;
using InternetBankCore.Services;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Reporting.Core.Validations;

public interface ITransactionStatisticsValidations
{
    Task<(decimal, decimal, decimal)> TotalIncomeFromTransactionsByMonth(int month);
    Task<(decimal, decimal, decimal)> TotalIncomeFromTransactionsByYear(int year);
    Task<decimal> TotalAtmDeposit();
    Task<(decimal, decimal, decimal)> AvgIncomeFromTransactionGelUsdEur();
}

public class TransactionStatisticsValidations : ITransactionStatisticsValidations
{
    private readonly ITransactionStatisticsRepository _transactionStatisticsRepository;
    private readonly ICurrencyService _currencyService;
    private readonly IAccountRepository _accountRepository;

    public TransactionStatisticsValidations(
        ITransactionStatisticsRepository transactionStatisticsRepository, 
        ICurrencyService currencyService, IAccountRepository accountRepository)
    {
        _transactionStatisticsRepository = transactionStatisticsRepository;
        _currencyService = currencyService;
        _accountRepository = accountRepository;
    }

    public async Task<(decimal, decimal, decimal)> TotalIncomeFromTransactionsByMonth(int month)
    {
        var transactions = _transactionStatisticsRepository.TransactionsLastMonths(month);
        if (transactions.Item1 == null)
        {
            throw new Exception("Could not find Inner Transactions");
        }

        if (transactions.Item2 == null)
        {
            throw new Exception("Could not find Outside Transactions");
        }

        var gelInnerIncome = await transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Fee);
        var gelOutsideIncome = await transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Fee);

        var otherInnerTransactions = await transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() != "gel")
            .ToListAsync();
        var otherOutsideTransactions = await transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .ToListAsync();

        foreach (var innerTransaction in otherInnerTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(innerTransaction.CurrencyCode, "Gel", innerTransaction.Amount);
                
            gelInnerIncome += convertedAmount;
        }
        
        foreach (var outsideTransaction in otherOutsideTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(outsideTransaction.CurrencyCode, "Gel", outsideTransaction.Amount);
        
            gelOutsideIncome += convertedAmount;
        }
        
        var gelTotalIncome = gelInnerIncome + gelOutsideIncome;
        return (gelInnerIncome, gelOutsideIncome, gelTotalIncome);
    }
    
    public async Task<(decimal, decimal, decimal)> TotalIncomeFromTransactionsByYear(int year)
    {
        var transactions = _transactionStatisticsRepository.TotalTransactionsLastYear(year);
        if (transactions.Item1 == null)
        {
            throw new Exception("Could not find Inner Transactions");
        }

        if (transactions.Item2 == null)
        {
            throw new Exception("Could not find Outside Transactions");
        }

        var gelInnerIncome = await transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Fee);
        var gelOutsideIncome = await transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Fee);

        var otherInnerTransactions = await transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() != "gel")
            .ToListAsync();
        var otherOutsideTransactions =  await transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .ToListAsync();

        foreach (var innerTransaction in otherInnerTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(innerTransaction.CurrencyCode, "Gel", innerTransaction.Amount);

            gelInnerIncome += convertedAmount;
        }

        foreach (var outsideTransaction in otherOutsideTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(outsideTransaction.CurrencyCode, "Gel", outsideTransaction.Amount);

            gelOutsideIncome += convertedAmount;
        }
        
        var gelTotalIncome = gelInnerIncome + gelOutsideIncome;
        return (gelInnerIncome, gelOutsideIncome, gelTotalIncome);
    }

    public async Task<decimal> TotalAtmDeposit()
    {
        var atmTransactions = _transactionStatisticsRepository.AtmTransactions();
        var gelAmount = await atmTransactions
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Amount);
        var otherTransactions = atmTransactions
            .Where(t => t.CurrencyCode.ToLower() != "gel");
        
        await Parallel.ForEachAsync(otherTransactions, async (otherTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(otherTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", otherTransaction.Amount);

            gelAmount += convertedAmount;
        });

        return gelAmount;
    }

    public async Task<(decimal, decimal, decimal)> AvgIncomeFromTransactionGelUsdEur()
    {
        var gel = await AvgIncomeFromTransactionGel();
        var usd = await AvgIncomeFromTransactionUsd();
        var eur = await AvgIncomeFromTransactionEur();

        return (gel, usd, eur);
    }

    private async Task<decimal> AvgIncomeFromTransactionGel()
    {
        var transactions = _transactionStatisticsRepository.GelAllTransactions();
        var gelAmount = await transactions
            .Where(t => t.CurrencyCode.ToLower() == "gel")
            .SumAsync(t => t.Amount);
        var otherTransactions = await transactions
            .Where(t => t.CurrencyCode.ToLower() != "gel")
            .ToListAsync();

        foreach (var otherTransaction in otherTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(otherTransaction.CurrencyCode, "Gel", otherTransaction.Fee);

            gelAmount += convertedAmount;
        }

        if (gelAmount == 0)
        {
            return 0;
        }
        
        return gelAmount / otherTransactions.Count;
    }
    
    private async Task<decimal> AvgIncomeFromTransactionUsd()
    {
        var transactions = _transactionStatisticsRepository.GelAllTransactions();
        var usdAmount = await transactions
            .Where(t => t.CurrencyCode.ToLower() == "usd")
            .SumAsync(t => t.Amount);
        var otherTransactions = await transactions
            .Where(t => t.CurrencyCode.ToLower() != "usd")
            .ToListAsync();
        
        foreach (var otherTransaction in otherTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(otherTransaction.CurrencyCode, "usd", otherTransaction.Fee);

            usdAmount += convertedAmount;
        }

        if (usdAmount == 0)
        {
            return 0;
        }
        return usdAmount / otherTransactions.Count;
    }
    
    private async Task<decimal> AvgIncomeFromTransactionEur()
    {
        var transactions = _transactionStatisticsRepository.GelAllTransactions();
        var eurAmount = await transactions
            .Where(t => t.CurrencyCode.ToLower() == "eur")
            .SumAsync(t => t.Amount);
        var otherTransactions = await transactions
            .Where(t => t.CurrencyCode.ToLower() != "eur")
            .ToListAsync();
        
        foreach (var otherTransaction in otherTransactions)
        {
            var convertedAmount = await _currencyService.ConvertAmount(otherTransaction.CurrencyCode, "eur", otherTransaction.Fee);

            eurAmount += convertedAmount;
        }

        if (eurAmount == 0)
        {
            return 0;
        }

        return eurAmount / otherTransactions.Count;
    }
}