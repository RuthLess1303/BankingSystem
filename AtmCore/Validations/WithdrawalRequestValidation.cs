namespace AtmCore.Validations;

public class WithdrawalRequestValidation
{
    public bool ValidateCreditCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber)) return false;

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
    // public bool ValidatePinCode(int pinCode)
    // {
    //     return pinCode is >= 1000 and <= 9999;
    // }
}