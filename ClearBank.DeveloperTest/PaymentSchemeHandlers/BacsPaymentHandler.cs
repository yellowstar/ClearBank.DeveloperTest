using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentSchemeHandlers
{
    public class BacsPaymentHandler : BasePaymentHandler, IPaymentSchemeHandler
    {
        public PaymentScheme SupportedScheme
        {
            get { return PaymentScheme.Bacs; }
        }

        public bool AccountSupported(Account account)
        {
            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}