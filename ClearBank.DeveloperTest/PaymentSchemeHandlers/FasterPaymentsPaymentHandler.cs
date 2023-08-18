using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentSchemeHandlers
{
    public class FasterPaymentsPaymentHandler : BasePaymentHandler, IPaymentSchemeHandler
    {
        public PaymentScheme SupportedScheme
        {
            get { return PaymentScheme.FasterPayments; }
        }

        public bool AccountSupported(Account account)
        {
            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments);
        }

        public override bool CreditCheck(Account account, decimal debitAmount)
        {
            return account.Balance >= debitAmount;
        }
    }
}