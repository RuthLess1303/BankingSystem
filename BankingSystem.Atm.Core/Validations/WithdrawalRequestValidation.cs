namespace BankingSystem.Atm.Core.Validations;

public interface IWithdrawalRequestValidation
{
    bool ValidateCreditCardNumber(string cardNumber);
    bool ValidatePinCode(string pinCode);
    void ValidateAmount(int amount);
}

public class WithdrawalRequestValidation : IWithdrawalRequestValidation
{
    private const int MinCardNumberLength = 13;
    private const int MaxCardNumberLength = 19;
    private const int PinCodeLength = 4;

    public bool ValidateCreditCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number is null, empty or whitespace.");

        if (!cardNumber.All(char.IsDigit))
            throw new ArgumentException("The card number can only contain digit characters.");

        if (cardNumber.Length is < MinCardNumberLength or > MaxCardNumberLength)
            throw new ArgumentException(
                $"Invalid card number length. Length should be between {MinCardNumberLength} and {MaxCardNumberLength}.");

        if (CalculateChecksum(cardNumber) % 10 != 0) throw new ArgumentException("Invalid card number.");

        return true;
    }

    public bool ValidatePinCode(string pinCode)
    {
        if (string.IsNullOrWhiteSpace(pinCode)) throw new ArgumentException("PIN code is null, empty or whitespace.");

        if (pinCode.Length != PinCodeLength)
            throw new ArgumentException($"Invalid PIN code length. Length should be {PinCodeLength}.");

        if (!pinCode.All(char.IsDigit)) throw new ArgumentException("PIN code must contain only digits.");

        return true;
    }

    public void ValidateAmount(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");
        }

        if (amount % 5 != 0)
        {
            throw new ArgumentException("Amount must be a multiple of 5.");
        }
    }

    private int CalculateChecksum(string cardNumber)
    {
        var reversedCardNumber = cardNumber.Reverse();
        int sum = 0, i = 0;

        foreach (var digitChar in reversedCardNumber)
        {
            if (!char.IsDigit(digitChar)) return 0;

            var digit = digitChar - '0';

            if ((i & 1) != 0)
            {
                digit <<= 1;

                if (digit > 9) digit -= 9;
            }

            sum += digit;
            i++;
        }

        return sum;
    }
}