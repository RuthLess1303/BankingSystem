using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using Newtonsoft.Json;

namespace InternetBank.Core.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertAmount(string fromCurrencyCode, string toCurrencyCode, decimal amount);
    Task<decimal> GetRateAsync(string currencyCode);

    Task<decimal> ConvertAmountBasedOnDate(TransactionEntity fromTransactionEntity, string toCurrencyCode, decimal amount);
}

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }
    
    public async Task<decimal> ConvertAmount(string fromCurrencyCode, string toCurrencyCode, decimal amount)
    {
        if (toCurrencyCode.ToUpper().Equals(fromCurrencyCode.ToUpper()))
        {
            return amount;
        }
        var toCurrency = await _currencyRepository.FindCurrency(toCurrencyCode);
        var toRate = toCurrency.RatePerQuantity;

        var fromCurrency = await _currencyRepository.FindCurrency(fromCurrencyCode);
        var fromRate = fromCurrency.RatePerQuantity;

        amount *= fromRate;
        amount /= toRate;

        return amount;
    }

    public async Task<decimal> ConvertAmountBasedOnDate(TransactionEntity fromTransactionEntity, string toCurrencyCode, decimal amount)
    {
        if (toCurrencyCode.Equals(fromTransactionEntity.CurrencyCode.ToUpper()))
        {
            return amount;
        }
        var toCurrency = await _currencyRepository.FindCurrencyWithDate(toCurrencyCode, fromTransactionEntity.TransactionTime);
        var toRate = toCurrency.RatePerQuantity;

        var fromCurrency = await _currencyRepository.FindCurrencyWithDate(fromTransactionEntity.CurrencyCode, fromTransactionEntity.TransactionTime);
        var fromRate = fromCurrency.RatePerQuantity;

        amount *= fromRate;
        amount /= toRate;

        return amount;
    }
    
    public async Task<decimal> GetRateAsync(string currencyCode)
    {
        var rate = await _currencyRepository.FindCurrency(currencyCode);

        return rate.Rate;
    }
}
