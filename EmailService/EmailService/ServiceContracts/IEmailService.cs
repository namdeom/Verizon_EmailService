using Services.CommonLibraries.Infrastructure.Faults;
using ADP.DS.ServiceEdge.Services.EmailService.FaultContracts;
using ADP.DS.ServiceEdge.Services.EmailService.MessageContracts;
using System.ServiceModel;

namespace ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts
{
    /// <summary>
    /// This is an interface to define the operations that will be exposed in the Email Service
    /// </summary>
    [ServiceContract(Namespace = SchemaNamespaces.EmailService)]
    public interface IEmailService
    {
        /// <summary>
        /// This endpoint is useful to send an email to one or more recipients with one or more attachments.
        /// </summary>
        /// <param name="request">A <see cref="SendEmailRequest"/>
        /// that contains the information to send an email.
        /// </param>
        /// <returns> Returns a <see cref="SendEmailResponse"/> that contains information if the email was sent successfully.</returns>
        [OperationContract]
        [FaultContract(typeof(SystemFault))]
        [FaultContract(typeof(AuthorizationFault))]
        [FaultContract(typeof(EmailServiceFault))]
        SendEmailResponse SendEmail(SendEmailRequest request);

        /// <summary>
        /// This endpoint is useful to send an email to one or more recipients with one or more attachments.
        /// </summary>
        /// <param name="request">A <see cref="SendEmailRequest"/>
        /// that contains the information to send an email.
        /// </param>
        /// <returns> Returns a <see cref="SendEmailWithBadAddressCheckResponse"/> that contains information if the email was sent successfully along with list of invalid Recipients.</returns>
        [OperationContract]
        [FaultContract(typeof(SystemFault))]
        [FaultContract(typeof(AuthorizationFault))]
        [FaultContract(typeof(EmailServiceFault))]
        SendEmailWithBadAddressCheckResponse SendEmailWithBadAddressCheck(SendEmailRequest request);
    }

}
