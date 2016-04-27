using Services.CommonLibraries.Infrastructure;
using ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts;
using ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider.Entities;
using ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider.Repository;
using AutoMapper;
using Common.Logging;
using GenericRepository;
using Microsoft.Practices.Unity;
using System.Diagnostics.CodeAnalysis;



namespace ADP.DS.ServiceEdge.Services.EmailService
{
    /// <summary>
    /// Factory that resolves the run time depoendencies for the smail service
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailServiceFactory : WcfServiceFactory
    {
        /// <summary>
        /// Registers the runtime dependencies. Registers dependencies like logging, auto mapper and other providers consumed runtime by the email service.
        /// </summary>
        /// <param name="container"><see cref="Microsoft.Practices.Unity"/> dependency injection container.</param>
        protected override void RegisterTypes(IUnityContainer container)
        {
            var adapter = container.Resolve<ILoggerFactoryAdapter>();
            container.RegisterInstance(typeof(ILog), adapter.GetLogger("EmailService"));
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IRepository<BadEmailDomain>, BadEmailDomainsRepository>();
            container.RegisterInstance(typeof(IMappingEngine), Mapper.Engine);
           
        }
    }
}