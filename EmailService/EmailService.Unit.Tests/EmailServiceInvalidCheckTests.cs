using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Services.CommonLibraries.Infrastructure.Faults;
using Services.CommonLibraries.Tests;
using Services.EmailService;
using Services.EmailService.FaultContracts;
using Services.EmailService.MessageContracts;
using AutoMapper;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.EmailServiceProvider.Interface;
using ES = Services.EmailService;

namespace EmailService.Unit.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EmailServiceInvalidCheckTests
    {
        private readonly Mock<ILog> _mockedLog = new Mock<ILog>();
        private Mock<IEmailServiceProvider> _mockEmailServiceProvider = new Mock<IEmailServiceProvider>();
        private Mock<IBadEmailDomainsCheckProvider> _mockBadEmailCheckProvider = new Mock<IBadEmailDomainsCheckProvider>();
        private static string _sessionToken = System.Configuration.ConfigurationManager.AppSettings["EmailServiceToken"];

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mapper.Reset();
            Mapper.AddProfile<EmailServiceMappingProfile>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Mapper.Reset();
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_EmptyAttachments()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = new List<FileAttachment>()
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_SMTPFormat1()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "TestBar<test@bar.com>",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_SMTPFormat2()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "\"Test(Bar)\"<test@bar.com>",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_All()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "bar",
                    Name = "foo"
                },
                 new FileAttachment
                {
                    Content = "bar1",
                    Name = "foo1"
                }

            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = null
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_Failure()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(false);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_Exception()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Throws(new ArgumentException());
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<SystemFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_InvalidSession_Null()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = null,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_InvalidSession_Empty()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = string.Empty,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_InvalidSession_Bad()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = "foo",
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput1()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = null,
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput2()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = string.Empty,
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput3()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "  ",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput4()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = null,
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput5()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = string.Empty,
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput6()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "  ",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput7()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = null,
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput8()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = string.Empty,
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput9()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "  ",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput10()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput11()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };


            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput12()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@barcom",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput13()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "testbar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };


            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput14()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = null,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput15()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string>();
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput16()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "testbar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput17()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com", "test@barcom" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };


            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput18()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com", "test" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };
            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput19()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "testbar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = null
            };


            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput20()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@barcom" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = null
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressReturnBadAddresslistCheck_InvalidInput21()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = null
            };


            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput22()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "bar",
                    Name = "foo"
                },
                new FileAttachment
                {
                    Content = null,
                    Name = "foo"
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput23()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = string.Empty,
                    Name = "foo"
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput24()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "     ",
                    Name = "foo"
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput25()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "foo",
                    Name = null
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput26()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "foo",
                    Name = string.Empty
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressThrowExceptionCheck_InvalidInput27()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "foo",
                    Name = "bar"
                },
                new FileAttachment
                {
                    Content = "foo",
                    Name = "   "
                }
            };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };
            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmailWithBadAddressCheck(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_InvalidDomain()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@Test.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.Is<string>( a => a.Contains("@Test.")))).Returns(true);
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "foo",
                    Name = "bar"
                },
                new FileAttachment
                {
                    Content = "foo",
                    Name = "bar"
                }
            };
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var response = emailservice.SendEmailWithBadAddressCheck(request);
            Assert.IsTrue(response.InvalidRecipients.BadDomains.Count == 1);
            Assert.AreEqual(response.InvalidRecipients.BadDomains[0], "test@test.com",true,CultureInfo.InvariantCulture);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_AllInvalidDomain()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@Test.com", "test@ABDLAWNET.COM" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@Test.com")).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@ABDLAWNET.COM")).Returns(true);
            var cclist = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>
            {
                new FileAttachment
                {
                    Content = "foo",
                    Name = "bar"
                },
                new FileAttachment
                {
                    Content = "foo",
                    Name = "bar"
                }
            };
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = attachments
            };

            var Invalidlist = emailservice.SendEmailWithBadAddressCheck(request);
            Assert.AreNotEqual(Invalidlist.InvalidRecipients.BadDomains.Count, 0);
            Assert.IsTrue(Invalidlist.IsSuccess == false);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_AllValidDomain()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com", "test@cdk.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmailWithBadAddressCheck(new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            });

            Assert.AreEqual(response.InvalidRecipients.BadDomains.Count, 0);
            Assert.IsNotNull(response.IsSuccess);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_ValidAndInvalidDomain()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com", "test@test.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@bar.com")).Returns(false);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@test.com")).Returns(true);
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = null,
                Attachments = null
            };

            var invalidlist = emailservice.SendEmailWithBadAddressCheck(request);
            Assert.AreNotEqual(invalidlist.InvalidRecipients.BadDomains.Count, 0);
            Assert.IsTrue(invalidlist.IsSuccess == true);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmailWithBadAddressCheck_ValidInputs_ValidAndInvalidDomainInCCList()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com", "test@bar.comm" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@bar.com")).Returns(false);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain("test@test.com")).Returns(true);
            var cclist = new List<string> { "test@bar.com", "test@test.com" };
            var request = new SendEmailRequest
            {
                SessionId = _sessionToken,
                Subject = "foo",
                Body = "bar",
                FromEmailAddress = "test@bar.com",
                Recipients = recipients,
                CarbonCopyList = cclist,
                Attachments = null
            };

            var invalidlist = emailservice.SendEmailWithBadAddressCheck(request);
            Assert.AreNotEqual(invalidlist.InvalidRecipients.BadDomains.Count, 0);
            Assert.IsTrue(invalidlist.IsSuccess == true);
        }
    }
}
