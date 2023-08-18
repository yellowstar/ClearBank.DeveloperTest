using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.PaymentSchemeHandlers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IEnumerable<IAccountDataStore> _accountDataStores;
        private readonly IEnumerable<IPaymentSchemeHandler> _paymentSchemeHandlers;
        private readonly IConfigurationSettings _configurationSettings;

        public PaymentService(IEnumerable<IAccountDataStore> accountDataStores, IEnumerable<IPaymentSchemeHandler> paymentSchemeHandlers, IConfigurationSettings configurationSettings)
        {
            _accountDataStores = accountDataStores;
            _configurationSettings = configurationSettings;
            _paymentSchemeHandlers = paymentSchemeHandlers;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var result = new MakePaymentResult() { Success = true };
            DataStoreType dataStoreType = Enum.TryParse<DataStoreType>(_configurationSettings.DataStoreType, true, out dataStoreType) ? dataStoreType : DataStoreType.Unknown;

            var accountDataStore = _accountDataStores.FirstOrDefault(s => s.DataStoreType == dataStoreType);

            if (accountDataStore == null)
            {
                result.Success = false;
                return result;
            }

            Account debtorAccount = accountDataStore.GetAccount(request.DebtorAccountNumber);
            Account creditorAccount = accountDataStore.GetAccount(request.CreditorAccountNumber);

            if (debtorAccount == null || creditorAccount == null)
            {
                result.Success = false;
                return result;
            }

            var paymentSchemeHandler = _paymentSchemeHandlers.FirstOrDefault(h => h.SupportedScheme == request.PaymentScheme);
            result.Success = paymentSchemeHandler.AccountSupported(debtorAccount) && paymentSchemeHandler.CreditCheck(debtorAccount, request.Amount);

            if (result.Success)
            {
                // Should be wrapped in an atomic transaction
                debtorAccount.Balance -= request.Amount;
                accountDataStore.UpdateAccount(debtorAccount);

                creditorAccount.Balance += request.Amount;
                accountDataStore.UpdateAccount(creditorAccount);
            }

            return result;
        }
    }
}