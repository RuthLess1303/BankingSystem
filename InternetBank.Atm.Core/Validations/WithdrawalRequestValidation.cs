namespace InternetBank.Atm.Core.Validations;

public interface IWithdrawalRequestValidation
{
    bool ValidateCreditCardNumber(string cardNumber);
    bool ValidateAmount(decimal amount);
    bool ValidatePinCode(string pinCode);
}

public class WithdrawalRequestValidation : IWithdrawalRequestValidation
{
    public bool ValidateCreditCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber)) return false;

        // Remove any non-digit characters from the card number
        cardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());
        
        // The card number must be between 13 and 19 digits long
        if (cardNumber.Length is < 13 or > 19) return false;
        
        var reversedCardNumber = cardNumber.Reverse();
        int sum = 0, i = 0;

        foreach (var digitChar in reversedCardNumber)
        {
            if (!char.IsDigit(digitChar)) return false;

            var digit = digitChar - '0';

            if ((i & 1) != 0)
            {
                digit <<= 1;

                if (digit > 9) digit -= 9;
            }

            sum += digit;
            i++;
        }

        return sum % 10 == 0;
    }

    public bool ValidateAmount(decimal amount)
    {
        return amount >= 0;
    }

    public bool ValidatePinCode(string pinCode)
    {
        return !string.IsNullOrWhiteSpace(pinCode) && pinCode.Length == 4 && pinCode.All(char.IsDigit);
    }
}