using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface;
using ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider.Repository;
using Microsoft.Practices.Unity;
using System.Diagnostics.CodeAnalysis;
using GenericRepository;
using ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider.Entities;

namespace ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider
{
    public class BadEmailDomainsProvider : IBadEmailDomainsCheckProvider
    {

        /// <summary>
        /// An instance of <see cref="IRepository{DealerCrossRef}"/> used to retrieve and store <see cref="DealerCrossRef"/>.
        /// </summary>
        private readonly IRepository<BadEmailDomain> _badEmailDomainRepository;

        public BadEmailDomainsProvider(IRepository<BadEmailDomain> badEmailDomainRepository)
        {
            _badEmailDomainRepository = badEmailDomainRepository;
        }

        /// <summary>
        /// function to get bad domain check
        /// </summary>
        /// <param name="eMailAddress">It contains complete email address.we need to fetch domain part from that</param>
        /// <returns></returns>
        public bool IsBadDomain(string eMailAddress)
        {
            if (string.IsNullOrEmpty(eMailAddress))
            {
                return true;
            }
            eMailAddress = eMailAddress.Trim();
            if(eMailAddress.IndexOf('@') <= 0) return true;
            string domain = eMailAddress.Substring(eMailAddress.IndexOf('@') + 1);
            if (string.IsNullOrEmpty(domain))
            {
                return true;
            }
            //var context = new ServiceEdgeCommonEntities();
            bool isBadDomain = _badEmailDomainRepository.All.Any(item => domain.Equals(item.Domain));
            return isBadDomain;
        }

        

    }

    
}
