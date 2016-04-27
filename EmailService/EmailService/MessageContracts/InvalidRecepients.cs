using System.Collections.Generic;
using System.ServiceModel;

namespace Services.EmailService.MessageContracts
{
    /// <summary>
    /// The output response for the <see cref="Services.EmailService.ServiceContracts.ISendEmailToValidAddressService.SendEmailWithBadAddressCheck"/> operation.
    /// </summary>
    [MessageContract(WrapperNamespace = SchemaNamespaces.EmailServiceMessage)]
    public class InvalidRecipients
    {
        public InvalidRecipients()
        {
            BadDomains = new List<string>();
        }

        /// <summary>
        /// Returns the <see cref="List<string>"/> contains email address with bad domains
        /// </summary>
        [MessageBodyMember]
        public List<string> BadDomains { get; set; }
    }
}