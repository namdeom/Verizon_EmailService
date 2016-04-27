using Services.EmailService.SmtpEmailServiceProvider.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Services.CommonLibraries.EntityFrameworkRepository;


namespace Services.EmailService.SmtpEmailServiceProvider.Repository
{

    public class BadEmailDomainsRepository : Repository<ServiceEdgeCommonEntities, BadEmailDomain>
    {

    }
    

}
