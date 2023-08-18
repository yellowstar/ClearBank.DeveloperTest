using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentSchemeHandlers
{
    public class ChapsPaymentHandler : BasePaymentHandler, IPaymentSchemeHandler
    {
        public PaymentScheme SupportedScheme
        {
            get { return PaymentScheme.Chaps; }
        }

        public bool AccountSupported(Account account)
        {
            return account.Status == AccountStatus.Live && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps);
        }
    }
}