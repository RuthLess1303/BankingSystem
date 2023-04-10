namespace InternetBank.Core.Validations;

public interface ITransactionValidations
{
    decimal CalculateFee(decimal amount, decimal feePercent, decimal additionalFee);
}

public class TransactionValidations : ITransactionValidations
{
    public decimal CalculateFee(decimal amount, decimal feePercent, decimal additionalFee)
    {
        return amount * feePercent / 100 + additionalFee;
    }
}