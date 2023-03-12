using InternetBankCore.Db;
using InternetBankCore.Db.Entities;
using Newtonsoft.Json;

namespace InternetBankCore.Services;

public interface ICurrencyService
{
    Task AddInDb();
    Task<decimal> ConvertAmount(string from, string to, decimal amount);
    Task<decimal> GetRate(string currencyCode);
}

public class CurrencyService : ICurrencyService
{
    private readonly AppDbContext _db;

    public CurrencyService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddInDb()
    {
        var currencies = GetCurrencies().Result;
        Parallel.ForEach(currencies, async currency => { await _db.AddAsync(currency); });

        await _db.SaveChangesAsync();
    }

    public async Task<decimal> ConvertAmount(string from, string to, decimal amount)
    {
        var toCurrency = await Task.Run(() => _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefault(c => c.Code == to));
        var toRate = toCurrency.Rate;

        if (toCurrency.Quantity != 1) toRate /= toCurrency.Quantity;

        if (from.ToUpper() == "GEL") amount /= toRate;
        var fromCurrency = await Task.Run(() => _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefault(c => c.Code == from));
        var fromRate = fromCurrency.Rate;

        if (fromCurrency.Quantity != 1) toRate /= fromCurrency.Quantity;

        amount *= fromRate;
        amount /= toRate;

        return amount;
    }

    public async Task<decimal> GetRate(string currencyCode)
    {
        var rate = await Task.Run(() => _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefault(c => c.Code == currencyCode));

        return rate.Rate;
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
}

public class Root
{
    public DateTime date { get; set; }
    public List<CurrencyEntity> currencies { get; } = new();
}