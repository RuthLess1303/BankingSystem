using InternetBank.Core.Validations;
using InternetBank.Db.Db.Repositories;
using Moq;

namespace InternetBank.Test;

public class PropertyValidationsTests
{
    private PropertyValidations _propertyValidations;
    
    [SetUp]
    public void Setup()
    {
        var currencyRepositoryMock = new Mock<ICurrencyRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var accountRepositoryMock = new Mock<IAccountRepository>();

        _propertyValidations = new PropertyValidations(currencyRepositoryMock.Object, userRepositoryMock.Object,
            accountRepositoryMock.Object);
    }
    
    [TestCase("55CD0360000031041898")]
    [TestCase("55GB0360000031041898")]
    [TestCase("GE550360000031041898")]
    [TestCase("AU550360000031041898")]
    [TestCase("")]
    [TestCase("123456")]
    [TestCase("GE55CD0360000031041!@#")]
    public void CheckIncorrectIbanFormat(string iban)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CheckIbanFormat(iban));
    }

    [TestCase("000")]
    [TestCase("100")]
    [TestCase("020")]
    public void CheckCorrectCvvFormatValidation(string cvv)
    {
        Assert.DoesNotThrow(() => _propertyValidations.CvvValidation(cvv));
    }
    
    [TestCase("000000")]
    [TestCase("")]
    [TestCase("0a0")]
    [TestCase("0A0")]
    [TestCase("dsh")]
    [TestCase("a00")]
    [TestCase("00a")]
    [TestCase("!22")]
    public void CheckIncorrectCvvFormatValidation(string cvv)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CvvValidation(cvv));
    }
    
    [TestCase("0003")]
    [TestCase("1001")]
    [TestCase("0200")]
    public void CheckCorrectPinFormatValidation(string pin)
    {
        Assert.DoesNotThrow(() => _propertyValidations.PinValidation(pin));
    }
    
    [TestCase("000000")]
    [TestCase("")]
    [TestCase("0a03")]
    [TestCase("0A01")]
    [TestCase("dsha")]
    [TestCase("!22@")]
    [TestCase("123")]
    public void CheckIncorrectPinFormatValidation(string pin)
    {
        Assert.Throws<Exception>(() => _propertyValidations.PinValidation(pin));
    }

    [TestCase("5/29/2015 5:50", true)]
    [TestCase("5/29/2005 12:52", true)]
    [TestCase("5/29/2025 22:25", false)]
    [TestCase("03/17/2023 10:12", true)]
    public void CheckCardExpirationDateValidation(DateTime date, bool result)
    {
        var isExpired = _propertyValidations.IsCardExpired(date);

        Assert.AreEqual(result, isExpired);
    }
    
    [TestCase("")]
    [TestCase("kjasuds")]
    [TestCase("test123@.com")]
    [TestCase("test123!gmail.com")]
    [TestCase("test123@gmail,com")]
    [TestCase("test123@")]
    public void CheckCorrectEmailDomainExistenceValidation(string email)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CheckEmailDomainExistence(email));
    }
    
    [TestCase("test123@gmail.com")]
    [TestCase("test123@mail.ru")]
    [TestCase("test123@bank.com")]
    [TestCase("test@gmail.com")]
    [TestCase("test123@gmail.cosin")]
    public void CheckIncorrectEmailDomainExistenceValidation(string email)
    {
        Assert.DoesNotThrow(() => _propertyValidations.CheckEmailDomainExistence(email));
    }
    
    [TestCase("Test123")]
    [TestCase("Test!@#$%")]
    [TestCase("SADS123!")]
    [TestCase("jjdsi123!")]
    [TestCase("Test1!")]
    [TestCase("")]
    public void CheckIfWeakPasswordIsDetected(string password)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CheckStrongPassword(password));
    }
    
    [TestCase("Test123!")]
    [TestCase("Tes@t153")]
    [TestCase("Str0ngPa$$w0rd")]
    public void CheckStrongPasswordDetection(string password)
    {
        Assert.DoesNotThrow(() => _propertyValidations.CheckStrongPassword(password));
    }
    
    [TestCase("1234567890123456")]
    [TestCase("123")]
    [TestCase("3300000aSd0")]
    [TestCase("3300000000a")]
    [TestCase("3300000000A")]
    [TestCase("3300000----")]
    [TestCase("")]
    public void CheckIncorrectPrivateNumberFormatDetection(string privateNumber)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CheckPrivateNumberFormat(privateNumber));
    }
    
    [TestCase("12345678901")]
    [TestCase("01000000001")]
    [TestCase("33000000000")]
    public void CheckCorrectPrivateNumberFormatDetection(string privateNumber)
    {
        Assert.DoesNotThrow(() => _propertyValidations.CheckPrivateNumberFormat(privateNumber));
    }

    [TestCase("hello123")]
    [TestCase("hello@")]
    [TestCase("123")]
    [TestCase("@#%$")]
    public void CheckIncorrectNameOrSurnameFormatDetection(string nameOrSurname)
    {
        Assert.Throws<Exception>(() => _propertyValidations.CheckNameOrSurname(nameOrSurname));
    }
    
    [TestCase("name")]
    [TestCase("TestName")]
    public void CheckCorrectNameOrSurnameFormatDetection(string nameOrSurname)
    {
        Assert.DoesNotThrow(() => _propertyValidations.CheckNameOrSurname(nameOrSurname));
    }
}