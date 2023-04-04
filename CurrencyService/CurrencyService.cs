using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Newtonsoft.Json;

namespace CurrencyService;

public interface ICurrencyService
{
    Task AddInDb();
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
        var currencies = await GetCurrencies();
        foreach (var currency in currencies) await _db.AddAsync(currency);

        await _db.SaveChangesAsync();
    }

    private async Task<List<CurrencyEntity>> GetCurrencies()
    {
        var client = new HttpClient();
        const string urlOfCurrencyJson = "https://nbg.gov.ge/gw/api/ct/monetarypolicy/currencies/ka/json/";
        var response = await client.GetAsync(urlOfCurrencyJson);

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
                ValidFromDate = currencyJson.ValidFromDate,
                RatePerQuantity = currencyJson.Rate / currencyJson.Quantity
            })
            .ToList();
    }
}

public class Root
{
    public DateTime Date { get; set; }
    public List<CurrencyEntity> currencies { get; } = new();
}