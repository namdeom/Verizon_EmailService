using Services.EmailService.SmtpEmailServiceProvider;
using Services.EmailServiceProvider.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text;
using Common.Logging;

namespace EmailServiceProvider.Unit.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SmtpEmailServiceProviderTests
    {
        private readonly Mock<ILog> _mockedLog = new Mock<ILog>();
        private Mock<ISmtpEmailService> _mockSmtpEmailService = new Mock<ISmtpEmailService>();
        private Mock<IBadEmailDomainsCheckProvider> _mockBadEmailDomainsCheckProvider = new Mock<IBadEmailDomainsCheckProvider>();

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            var returnValue= Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_DefaultSmptpService()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object);
            Assert.IsNotNull(emailservice);
        }


        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object,_mockSmtpEmailService.Object);
            var recipients = new List<string> {"test@bar.com"};
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        //Bd Domain Test Cases
        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_GoodDomain_WithDefault()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_BadDomain_WithDefault()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@ameruroproducts.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_SMTPFormat1()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "testBar<test@bar.com>", recipients, null, null));
        }


        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_SMTPFormat2()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "\"testBar(test)\"<test@bar.com>", recipients, null, null));
        }


        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_HTMLBody()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "<html>Foo</html>", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withCC()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withAttachments()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = EncodeTo64("foo"), Name = "bar" });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsTrue(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_GeneralException()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>())).Throws(new Exception());
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_OtherException()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>())).Throws(new ArgumentNullException());
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_NullSubject()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail(null, "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_EmptySubject()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("      ", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_NullBody()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", null, "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_EmptyBody()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", string.Empty, "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_NullFrom()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", null, recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_EmptyFrom()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "    ", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailFrom1()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailFrom2()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailFrom3()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailFrom4()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailFrom5()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar@com", recipients, null, null));
        }


        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailTo1()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@barcom" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailTo2()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
           _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", null, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailTo3()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string>();
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailTo4()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com", "foobar" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailCC1()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@barcom" };
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, cclist, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_InvalidInput_InvalidEmailCC2()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var cclist = new List<string> { "test@bar.com" , "foo@com"};
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, cclist, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withInvalidAttachments()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = "foo", Name = "bar" });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withInvalidAttachments2()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = EncodeTo64("foo"), Name = null });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withInvalidAttachments3()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = EncodeTo64("foo"), Name = "  " });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withInvalidAttachments4()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = null, Name = "bar" });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }

        [TestMethod, TestCategory("Unit")]
        public void SendEmail_ValidInputs_withInvalidAttachments5()
        {
            var emailservice = new SmtpEmailService(_mockedLog.Object, _mockSmtpEmailService.Object);
            var recipients = new List<string> { "test@bar.com" };
            var attachments = new List<FileAttachment>();
            attachments.Add(new FileAttachment { Content = "  ", Name = "bar" });
            _mockSmtpEmailService.Setup(m => m.Send(It.IsAny<MailMessage>()));
            Assert.IsFalse(emailservice.SendEmail("foo", "bar", "test@bar.com", recipients, recipients, attachments));
        }
    }
}
