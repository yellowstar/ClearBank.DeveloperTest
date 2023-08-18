using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentSchemeHandlers
{
    public interface IPaymentSchemeHandler
    {
        PaymentScheme SupportedScheme { get; }

        bool AccountSupported(Account account);

        bool CreditCheck(Account account, decimal debitAmount);
    }
}