using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.PaymentSchemeHandlers
{
    public class BasePaymentHandler
    {
        public virtual bool CreditCheck(Account account, decimal debitAmount)
        {
            return true;
        }
    }
}