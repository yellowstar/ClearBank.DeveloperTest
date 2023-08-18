using ClearBank.DeveloperTest.Configuration;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.PaymentSchemeHandlers;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Tests
{
    [TestFixture]
    internal class AccountDataStoreTests
    {
        private Mock<IConfigurationSettings> _mockConfigurationSettings;
        private Mock<IAccountDataStore> _mockAccountDataStore;
        private Mock<IAccountDataStore> _mockBackupAccountDataStore;
        private List<IAccountDataStore> _accountDataStores;
        private List<IPaymentSchemeHandler> _paymentSchemeHandlers;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationSettings = new Mock<IConfigurationSettings>(MockBehavior.Strict);
            _mockAccountDataStore = new Mock<IAccountDataStore>(MockBehavior.Strict);
            _mockAccountDataStore.SetupGet(s => s.DataStoreType).Returns(DataStoreType.Main);
            _mockBackupAccountDataStore = new Mock<IAccountDataStore>(MockBehavior.Strict);
            _mockBackupAccountDataStore.SetupGet(b => b.DataStoreType).Returns(DataStoreType.Backup);
            _accountDataStores = new List<IAccountDataStore>() { _mockAccountDataStore.Object, _mockBackupAccountDataStore.Object };
            _paymentSchemeHandlers = new List<IPaymentSchemeHandler>();
        }

        [Test]
        public void WhenGivenUnknownDataStoreType_ThenPaymentServiceShouldReturnNonSuccess()
        {
            //Arrange
            _mockConfigurationSettings.SetupGet(c => c.DataStoreType).Returns("Unknown");

            _paymentSchemeHandlers.Add(new FasterPaymentsPaymentHandler());

            var paymentService = new PaymentService(_accountDataStores, _paymentSchemeHandlers, _mockConfigurationSettings.Object);
            var makePaymentRequest = new MakePaymentRequest()
            {
                CreditorAccountNumber = "1234567890",
                DebtorAccountNumber = "987654321",
                Amount = 246.80m,
                PaymentDate = DateTime.Today,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            //Act
            var paymentServiceResult = paymentService.MakePayment(makePaymentRequest);

            //Assert
            paymentServiceResult.Success.Should().BeFalse();
        }

        [Test]
        [TestCase("Main", 2, 0)]
        [TestCase("Backup", 0, 2)]
        public void WhenGivenMainDataStoreType_ThenAccountDataStoreShouldBeUsed(string dataStoreType, int mainStoreInvocationCount, int backupStoreInvocationCount)
        {
            //Arrange
            _mockConfigurationSettings.SetupGet(c => c.DataStoreType).Returns(dataStoreType);
            _mockAccountDataStore.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns((Account)null);
            _mockBackupAccountDataStore.Setup(b => b.GetAccount(It.IsAny<string>()))
                .Returns((Account)null);
            _paymentSchemeHandlers.Add(new FasterPaymentsPaymentHandler());
            var paymentService = new PaymentService(_accountDataStores, _paymentSchemeHandlers, _mockConfigurationSettings.Object);
            var makePaymentRequest = new MakePaymentRequest()
            {
                CreditorAccountNumber = "1234567890",
                DebtorAccountNumber = "987654321",
                Amount = 246.80m,
                PaymentDate = DateTime.Today,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            //Act
            var paymentServiceResult = paymentService.MakePayment(makePaymentRequest);

            //Assert
            _mockAccountDataStore.Verify(s => s.GetAccount(It.IsAny<string>()), Times.Exactly(mainStoreInvocationCount));
            _mockBackupAccountDataStore.Verify(b => b.GetAccount(It.IsAny<string>()), Times.Exactly(backupStoreInvocationCount));
            paymentServiceResult.Success.Should().BeFalse();
        }

        [Test]
        public void WhenGivenMainDataStoreTypeAndValidAccounts_ThenPaymentServiceShouldReturnSuccess()
        {
            //Arrange
            var mockStore = new List<Account>
            {
                new Account()
                {
                    AccountNumber = "1234567890",
                    Balance = 100m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
                },
                new Account()
                                {
                    AccountNumber = "987654321",
                    Balance = 500m,
                    Status = AccountStatus.Live,
                    AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
                }
            };
            _mockConfigurationSettings.SetupGet(c => c.DataStoreType).Returns("Main");
            _mockAccountDataStore.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns((string accountNo) =>
                {
                    return mockStore.FirstOrDefault(a => a.AccountNumber == accountNo);
                });
            _mockAccountDataStore.Setup(s => s.UpdateAccount(It.IsAny<Account>()));
            _mockBackupAccountDataStore.Setup(b => b.GetAccount(It.IsAny<string>()))
                .Returns((Account)null);
            _paymentSchemeHandlers.Add(new BacsPaymentHandler());
            var paymentService = new PaymentService(_accountDataStores, _paymentSchemeHandlers, _mockConfigurationSettings.Object);
            var makePaymentRequest = new MakePaymentRequest()
            {
                CreditorAccountNumber = "1234567890",
                DebtorAccountNumber = "987654321",
                Amount = 246.80m,
                PaymentDate = DateTime.Today,
                PaymentScheme = PaymentScheme.Bacs
            };

            //Act
            var paymentServiceResult = paymentService.MakePayment(makePaymentRequest);

            // Assert
            paymentServiceResult.Success.Should().BeTrue();
        }
    }
}