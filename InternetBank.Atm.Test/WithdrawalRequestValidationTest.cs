using InternetBank.Atm.Core.Validations;

namespace InternetBank.Atm.Test;

[TestFixture]
public class WithdrawalRequestValidationTests
{
    [SetUp]
    public void SetUp()
    {
        _withdrawalValidation = new WithdrawalRequestValidation();
    }

    private WithdrawalRequestValidation _withdrawalValidation;

    [TestCase("1234567890123456")]
    [TestCase("12341234123412341")]
    [TestCase("123412341234")]
    [TestCase("4111111111111112")]
    [TestCase("123456789012345")]
    [TestCase("abcd123456789012")]
    [TestCase("5559955555554444")]
    [TestCase("123456")]
    [TestCase("4111 1111 1111 111a")]
    [TestCase("")]
    [TestCase(null)]
    [TestCase("411111111111!!!!")]
    public void ValidateCreditCardNumber_InvalidInput_ReturnsFalse(string input)
    {
        var result = _withdrawalValidation.ValidateCreditCardNumber(input);

        Assert.That(result, Is.False);
    }

    [TestCase("4111111111111111")]
    [TestCase("5105105105105100")]
    [TestCase("378282246310005")]
    [TestCase("6011000990139424")]
    [TestCase("30569309025904")]
    [TestCase("3530111333300000")]
    [TestCase("5555555555554444")]
    [TestCase("4111111111111111")]
    [TestCase("4012888888881881")]
    [TestCase("4222222222222")]
    [TestCase("5555555555554444")]
    public void ValidateCreditCardNumber_ValidInput_ReturnsTrue(string input)
    {
        var result = _withdrawalValidation.ValidateCreditCardNumber(input);

        Assert.That(result, Is.True);
    }
}