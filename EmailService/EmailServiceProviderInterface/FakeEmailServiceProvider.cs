using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface
{
    /// <summary>
    /// This is an implementation of the <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.IEmailServiceProvider"/>
    /// used mainly for unit testing purpose.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeEmailServiceProvider : IEmailServiceProvider
    {
        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="subject">A <see cref="string"/> that contains the email subject.</param>
        /// <param name="body">A <see cref="string"/> that contains the message body. It can contain HTML content.</param>
        /// <param name="fromEmailAddress">A <see cref="string"/> that contains a valid email address of the sender.</param>
        /// <param name="recipients">A <see cref="List{T}"/> of  <see cref="string"/> that contains list of email addresses of one or more recipients.</param>
        /// <param name="ccList">A <see cref="List{T}"/> of <see cref="string"/> that contains list of email addresses of one or more CC recipients. This is optional.</param>
        /// <param name="attachments">A <see cref="List{T}"/> of <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/> attachments. This is optional.</param>
        /// <returns>Returns <see langword="true"/> if the email was sent successfully else <see langword="false"/>.</returns>
        public bool SendEmail(string subject, string body, string fromEmailAddress, IList<string> recipients, IList<string> ccList,
            IList<FileAttachment> attachments)
        {
            return true;
        }
    }
}
