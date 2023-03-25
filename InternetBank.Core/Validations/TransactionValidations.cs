namespace InternetBank.Core.Validations;

public interface ITransactionValidations
{
    decimal CalculateGrossAmount(decimal amount, decimal feePercent, decimal additionalFee);
    decimal CalculateFee(decimal amount, decimal feePercent, decimal additionalFee);
}

public class TransactionValidations : ITransactionValidations
{
    public decimal CalculateGrossAmount(decimal amount, decimal feePercent, decimal additionalFee)
    {
        return amount * (feePercent + 100) / 100 + additionalFee;
    }
    
    public decimal CalculateFee(decimal amount, decimal feePercent, decimal additionalFee)
    {
        return amount * feePercent / 100 + additionalFee;
    }
}