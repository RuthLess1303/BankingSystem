using BankingSystemSharedDb.Db.Repositories;
using InternetBankCore.Validations;
using Moq;

namespace InternetBankTest;

public class TransactionValidationsTests
{
    private TransactionValidations _transactionValidations;
    
    [SetUp]
    public void Setup()
    {
        _transactionValidations = new TransactionValidations();
    }

    [TestCase(329.12, 1, 0.5, 332.9112)]
    [TestCase(1092.128882, 6, 0.5, 1158.15661492)]
    [TestCase(100, 22, 8.5, 130.5)]
    [TestCase(0.1, 15, 0.5, 0.615)]
    public void CheckGrossAmountCalculation(decimal amount, decimal feePercent, decimal additionalFee, decimal expected)
    {
        decimal grossAmount = _transactionValidations.CalculateGrossAmount(amount,feePercent,additionalFee);
        
        Assert.AreEqual(expected, grossAmount);
    }
    
    [TestCase(100, 1, 0.5, 1.5)]
    [TestCase(32.9878, 98, 25, 57.328044)]
    [TestCase(998.123, 1.23, 12, 24.2769129)]
    public void CheckFeeCalculation(decimal amount, decimal feePercent, decimal additionalFee, decimal expected)
    {
        decimal fee = _transactionValidations.CalculateFee(amount,feePercent,additionalFee);
        
        Assert.AreEqual(expected, fee);
    }
}