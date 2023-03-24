using InternetBank.Atm.Core.Repositories;
using InternetBank.Atm.Core.Services;
using Moq;

namespace InternetBank.Core.Test;

public class WithdrawalServiceTests
{
    private WithdrawalService _withdrawalService;
    
    [SetUp]
    public void Setup()
    {
        var cardAuthServiceMock = new Mock<ICardAuthService>();
        var transactionRepositoryMock = new Mock<ITransactionRepository>();
        
        _withdrawalService = new WithdrawalService(cardAuthServiceMock.Object, transactionRepositoryMock.Object);
    }
    
    [TestCase(100, 2)]
    [TestCase(2346.0361, 46.920722)]
    [TestCase(695.316, 13.90632)]
    [TestCase(20, 0.4)]
    [TestCase(1, 0.02)]
    
    public void CheckCalculateFee(decimal amount, decimal expected)
    {
        var actual = _withdrawalService.CalculateFee(amount);
        
        Assert.AreEqual(expected, actual);
    }
    
    [TestCase(100, 2, 102)]
    [TestCase(0, 0, 0)]
    [TestCase(18829.009328, 413.960632,19242.96996)]
    [TestCase(20, 0.4, 20.4)]
    [TestCase(1, 0, 1)]
    
    public void CheckCalculateWithdrawAmount(decimal amount, decimal fee, decimal expected)
    {
        var actual = _withdrawalService.CalculateWithdrawAmount(amount, fee);
        
        Assert.AreEqual(expected, actual);
    }
}