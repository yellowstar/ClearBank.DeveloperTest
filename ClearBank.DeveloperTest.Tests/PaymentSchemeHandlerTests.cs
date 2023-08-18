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
    public class PaymentSchemeHandlerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, TestCaseSource(typeof(PaymentSchemeHandlerTestCaseFactory), "ValidPaymentSchemeCases")]
        public bool GivenCorrectPaymentScheme_WhenChecked_SchemeHandlerReturnsTrue(IPaymentSchemeHandler schemeHandler, Account account)
        {
            return schemeHandler.AccountSupported(account);
        }

        [Test, TestCaseSource(typeof(PaymentSchemeHandlerTestCaseFactory), "InvalidPaymentSchemeCases")]
        public bool GivenInCorrectPaymentScheme_WhenChecked_SchemeHandlerReturnsFalse(IPaymentSchemeHandler schemeHandler, Account account)
        {
            return schemeHandler.AccountSupported(account);
        }
    }
}