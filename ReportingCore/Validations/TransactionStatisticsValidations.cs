using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Repositories;
using InternetBankCore.Services;
using Microsoft.EntityFrameworkCore;
using ReportingCore.Repositories;

namespace ReportingCore.Validations;

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

        var otherInnerTransactions = transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() != "gel");
        var otherOutsideTransactions =  transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel");

        await Parallel.ForEachAsync(otherInnerTransactions, async (innerTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(innerTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", innerTransaction.Amount);

            gelInnerIncome += convertedAmount;
        });
        
        await Parallel.ForEachAsync(otherOutsideTransactions, async (outsideTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(outsideTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", outsideTransaction.Amount);

            gelOutsideIncome += convertedAmount;
        });
        
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

        var otherInnerTransactions = transactions.Item1
            .Where(t => t.CurrencyCode.ToLower() != "gel");
        var otherOutsideTransactions =  transactions.Item2
            .Where(t => t.CurrencyCode.ToLower() == "gel");

        await Parallel.ForEachAsync(otherInnerTransactions, async (innerTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(innerTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", innerTransaction.Amount);

            gelInnerIncome += convertedAmount;
        });
        
        await Parallel.ForEachAsync(otherOutsideTransactions, async (outsideTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(outsideTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", outsideTransaction.Amount);

            gelOutsideIncome += convertedAmount;
        });
        
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
        var otherTransactions = transactions
            .Where(t => t.CurrencyCode.ToLower() != "gel");
        var transactionsList = await transactions.ToListAsync();
        
        await Parallel.ForEachAsync(otherTransactions, async (otherTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(otherTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "Gel", otherTransaction.Amount);

            gelAmount += convertedAmount;
        });

        return gelAmount / transactionsList.Count;
    }
    
    private async Task<decimal> AvgIncomeFromTransactionUsd()
    {
        var transactions = _transactionStatisticsRepository.GelAllTransactions();
        var usdAmount = await transactions
            .Where(t => t.CurrencyCode.ToLower() == "usd")
            .SumAsync(t => t.Amount);
        var otherTransactions = transactions
            .Where(t => t.CurrencyCode.ToLower() != "usd");
        var transactionsList = await transactions.ToListAsync();
        
        await Parallel.ForEachAsync(otherTransactions, async (otherTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(otherTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "usd", otherTransaction.Amount);

            usdAmount += convertedAmount;
        });

        return usdAmount / transactionsList.Count;
    }
    
    private async Task<decimal> AvgIncomeFromTransactionEur()
    {
        var transactions = _transactionStatisticsRepository.GelAllTransactions();
        var eurAmount = await transactions
            .Where(t => t.CurrencyCode.ToLower() == "eur")
            .SumAsync(t => t.Amount);
        var otherTransactions = transactions
            .Where(t => t.CurrencyCode.ToLower() != "eur");
        var transactionsList = await transactions.ToListAsync();
        
        await Parallel.ForEachAsync(otherTransactions, async (otherTransaction, ct) =>
        {
            var fromCurrency = await _accountRepository.GetAccountCurrencyCode(otherTransaction.AggressorIban);
            var convertedAmount = await _currencyService.ConvertAmount(fromCurrency, "eur", otherTransaction.Amount);

            eurAmount += convertedAmount;
        });

        return eurAmount / transactionsList.Count;
    }
}