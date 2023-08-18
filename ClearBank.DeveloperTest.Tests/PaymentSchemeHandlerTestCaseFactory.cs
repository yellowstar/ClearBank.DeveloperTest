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
    public class PaymentSchemeHandlerTestCaseFactory
    {
        public static IEnumerable<TestCaseData> ValidPaymentSchemeCases
        {
            get
            {
                yield return new TestCaseData(new BacsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs }).Returns(true);
                yield return new TestCaseData(new ChapsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Live }).Returns(true);
                yield return new TestCaseData(new FasterPaymentsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments }).Returns(true);
            }
        }

        public static IEnumerable<TestCaseData> InvalidPaymentSchemeCases
        {
            get
            {
                yield return new TestCaseData(new BacsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps }).Returns(false);
                yield return new TestCaseData(new ChapsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Status = AccountStatus.Live }).Returns(false);
                yield return new TestCaseData(new ChapsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Disabled }).Returns(false);
                yield return new TestCaseData(new FasterPaymentsPaymentHandler(), new Account() { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs }).Returns(false);
            }
        }
    }
}