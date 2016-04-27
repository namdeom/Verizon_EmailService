using Services.EmailService.SmtpEmailServiceProvider;
using Services.EmailServiceProvider.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text;
using System.Linq;
using Common.Logging;
using Services.EmailService.SmtpEmailServiceProvider.Repository;
using GenericRepository;
using Services.EmailService.SmtpEmailServiceProvider.Entities;

namespace EmailServiceProvider.Unit.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BadDomainCheckProviderTests
    {
        private Mock<IRepository<BadEmailDomain>> _mockBadDomainRepository = new Mock<IRepository<BadEmailDomain>>();

       
        [TestMethod, TestCategory("Unit")]
        public void CheckDomain_BadDomainService_GoodDomain()
        {
            var baddomain = new BadEmailDomainsProvider(_mockBadDomainRepository.Object);
            var domains = new List<BadEmailDomain>();
            domains.Add(new BadEmailDomain { Domain = "bar.com" });
            domains.Add(new BadEmailDomain { Domain = "bad.com" });
            _mockBadDomainRepository.Setup(m => m.All).Returns(domains.AsQueryable);
            string recipient = "test@foo.com";
            Assert.IsFalse(baddomain.IsBadDomain(recipient));
        }


        [TestMethod, TestCategory("Unit")]
        public void CheckDomain_BadDomainService_BadDomain()
        {
            var baddomain = new BadEmailDomainsProvider(_mockBadDomainRepository.Object);
            var domains = new List<BadEmailDomain>();
            domains.Add(new BadEmailDomain { Domain = "bar.com" });
            domains.Add(new BadEmailDomain { Domain = "bad.com" });
            _mockBadDomainRepository.Setup(m => m.All).Returns(domains.AsQueryable);
            string recipient = "test@bad.com";
            Assert.IsTrue(baddomain.IsBadDomain(recipient));
        }

        [TestMethod, TestCategory("Unit")]
        public void CheckDomain_BadDomainServiceNullEmpty()
        {
            var baddomain = new BadEmailDomainsProvider(_mockBadDomainRepository.Object);
            Assert.IsTrue(baddomain.IsBadDomain(null));
            Assert.IsTrue(baddomain.IsBadDomain(string.Empty));
            Assert.IsTrue(baddomain.IsBadDomain("    "));

            Assert.IsTrue(baddomain.IsBadDomain("test"));
            Assert.IsTrue(baddomain.IsBadDomain("test@"));
        
        }

    }
}
