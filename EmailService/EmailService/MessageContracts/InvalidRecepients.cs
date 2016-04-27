using System.Collections.Generic;
using System.ServiceModel;

namespace ADP.DS.ServiceEdge.Services.EmailService.MessageContracts
{
    /// <summary>
    /// The output response for the <see cref="ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts.ISendEmailToValidAddressService.SendEmailWithBadAddressCheck"/> operation.
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