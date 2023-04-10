using InternetBank.Core.Validations;

namespace InternetBank.Test;

public class TransactionValidationsTests
{
    private TransactionValidations _transactionValidations;
    
    [SetUp]
    public void Setup()
    {
        _transactionValidations = new TransactionValidations();
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