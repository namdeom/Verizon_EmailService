using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.EmailServiceProvider.Interface
{
    public interface IBadEmailDomainsCheckProvider
    {
        /// <summary>
        /// function to validate email address domain
        /// </summary>
        /// <param name="eMailAddress"></param>
        /// <returns></returns>
        bool IsBadDomain(string eMailAddress);
    }
}
