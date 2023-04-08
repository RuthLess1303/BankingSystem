using InternetBank.Db.Db.Repositories;

namespace InternetBank.Core.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertAmount(string fromCurrencyCode, string toCurrencyCode, decimal amount);
    Task<decimal> GetRateAsync(string currencyCode);
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
    
    public async Task<decimal> GetRateAsync(string currencyCode)
    {
        var rate = await _currencyRepository.FindCurrency(currencyCode);
        if (rate == null)
        {
            return 1;
        }

        return rate.Rate;
    }
}
