using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using Newtonsoft.Json;

namespace InternetBank.Core.Services;

public interface ICurrencyService
{
    Task AddInDb();
    Task<decimal> ConvertAmount(string from, string to, decimal amount);
    Task<decimal> GetRateAsync(string currencyCode);
}

public class CurrencyService : ICurrencyService
{
    private readonly AppDbContext _db;
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyService(
        AppDbContext db, 
        ICurrencyRepository currencyRepository)
    {
        _db = db;
        _currencyRepository = currencyRepository;
    }

    private async Task<List<CurrencyEntity>> GetCurrencies()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/ka/json/");

        var contentString = await response.Content.ReadAsStringAsync();

        var dataList = JsonConvert.DeserializeObject<Root[]>(contentString);
        var currencyList = dataList![0].currencies;

        return currencyList.Select(currencyJson => new CurrencyEntity
            {
                Code = currencyJson.Code,
                Quantity = currencyJson.Quantity,
                RateFormatted = currencyJson.RateFormatted,
                DiffFormatted = currencyJson.DiffFormatted,
                Rate = currencyJson.Rate,
                Name = currencyJson.Name,
                Diff = currencyJson.Diff,
                Date = currencyJson.Date,
                ValidFromDate = currencyJson.ValidFromDate
            })
            .ToList();
    }

    public async Task AddInDb()
    {
        var currencies = await GetCurrencies();
        foreach (var currency in currencies)
        {
            await _db.AddAsync(currency);
        }
        
        await _db.SaveChangesAsync();
    }
    
    public async Task<decimal> ConvertAmount(string from, string to, decimal amount)
    {
        if (to.Equals("GEL", StringComparison.OrdinalIgnoreCase))
        {
            var fromCurrencyEntity = await _currencyRepository.FindCurrency(from);

            if (fromCurrencyEntity == null)
            {
                throw new ArgumentException($"Currency {from} not found");
            }

            var rate = decimal.Divide(fromCurrencyEntity.Rate, fromCurrencyEntity.Quantity);

            amount *= rate;
            
            return amount;
        }

        var toCurrency = await _currencyRepository.FindCurrency(to);

        if (toCurrency == null)
        {
            throw new ArgumentException($"Currency {to} not found");
        }

        var toRate = toCurrency.Rate;

        if (toCurrency.Quantity != 1)
        {
            toRate = decimal.Divide(toCurrency.Rate, toCurrency.Quantity);
        }
        
        if (from.Equals("GEL", StringComparison.OrdinalIgnoreCase))
        {
            amount = decimal.Divide(amount, toRate);
            return amount;
        }

        var fromCurrency = await _currencyRepository.FindCurrency(from);

        if (fromCurrency == null)
        {
            throw new ArgumentException($"Currency {from} not found");
        }

        var fromRate = fromCurrency.Rate;

        if (fromCurrency.Quantity != 1)
        {
            fromRate = decimal.Divide(fromCurrency.Rate, fromCurrency.Quantity);
        }

        amount *= fromRate;
        amount = decimal.Divide(amount, toRate);

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


public class Root
{
    public DateTime date { get; set; }
    public List<CurrencyEntity> currencies { get; } = new();
}