using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class EmailServiceTests
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
        public void SendEmail_ValidInputs()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_EmptyAttachments()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_SMTPFormat1()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_SMTPFormat2()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(true);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_All()
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
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_Failure()
        {
            var emailservice = new ES.EmailService(_mockedLog.Object, Mapper.Engine, _mockEmailServiceProvider.Object, _mockBadEmailCheckProvider.Object, new ES.FakeUserProvider());
            var recipients = new List<string> { "test@bar.com" };
            _mockEmailServiceProvider.Setup(m => m.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<List<FileAttachment>>())).Returns(false);
            _mockBadEmailCheckProvider.Setup(m => m.IsBadDomain(It.IsAny<string>())).Returns(false);
            var response = emailservice.SendEmail(new SendEmailRequest
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
        public void SendEmail_ValidInputs_Exception()
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

            var fault = ExceptionAssert.Throws<FaultException<SystemFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_InvalidSession_Null()
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

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_InvalidSession_Empty()
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

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_InvalidSession_Bad()
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

            var fault = ExceptionAssert.Throws<FaultException<AuthorizationFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput1()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput2()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput3()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput4()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput5()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput6()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput7()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput8()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput9()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput10()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput11()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput12()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput13()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput14()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput15()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput16()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput17()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput18()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput19()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput20()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput21()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput22()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput23()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput24()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput25()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput26()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput27()
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

            var fault = ExceptionAssert.Throws<FaultException<EmailServiceFault>>(() => emailservice.SendEmail(request));
            Assert.IsNotNull(fault);
            Assert.IsTrue(fault.Detail == EmailServiceFault.ValidationFailed);
        }

       
    }
}
