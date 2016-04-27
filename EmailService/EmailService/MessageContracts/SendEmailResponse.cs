using System.ServiceModel;

namespace Services.EmailService.MessageContracts
{
    /// <summary>
    /// The output response for the <see cref="Services.EmailService.ServiceContracts.IEmailService.SendEmail"/> operation.
    /// </summary>
    [MessageContract(WrapperNamespace = SchemaNamespaces.EmailServiceMessage)]
    public class SendEmailResponse
    {
        /// <summary>
        /// Returns the <see cref="bool"/> to indicate if email was sent successfully.
        /// </summary>
        [MessageBodyMember]
        public bool IsSuccess { get; set; }
    }

        /// <summary>
    /// The output response for the <see cref="Services.EmailService.ServiceContracts.IEmailService.SendEmailWithBadAddressCheck"/> operation.
    /// </summary>
    [MessageContract(WrapperNamespace = SchemaNamespaces.EmailServiceMessage)]
    public class SendEmailWithBadAddressCheckResponse
    {
        /// <summary>
        /// Returns the <see cref="bool"/> to indicate if email was sent successfully.
        /// </summary>
        [MessageBodyMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Returns the <see cref="InvalidRecipients"/> to indicate if email contains Invalid Recipients.
        /// </summary>
        [MessageBodyMember]
        public InvalidRecipients InvalidRecipients { get; set; }
    }
    
}