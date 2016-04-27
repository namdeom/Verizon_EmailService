using System.Collections.Generic;

namespace Services.EmailServiceProvider.Interface
{
    /// <summary>
    /// This interface is used to abstract the actual implementation away from the email service. 
    /// There must be a concrete implementation of this interface registered with the Unity IoC container
    /// </summary>
    public interface IEmailServiceProvider
    {
        
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="fromEmailAddress"/>, <paramref name="recipients"/> and <paramref name="ccList"/> should contain valid SMTP email address in proper format as below:
        /// <list type="bullet">
        /// <item>
        /// <description>The email format is displayName&lt;email address&gt;</description>
        /// </item>
        /// <item>
        /// <description>Display name is optional. By default only email address will be provided in the string.</description>
        /// </item>
        /// <item>
        /// <description>If display name is provided, the email address should be within &lt;&gt;.</description>
        /// </item>
        /// <item>
        /// <description>If the display name contains characters that have special meaning in SMTP addresses, such as &lt; or @, the entire display name should be enclosed in double quotes.</description>
        /// </item>
        /// <item>
        /// <description>If the total character exceeds 255, display name should be dropped and only email address should be provided without &lt;&gt;.</description>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="subject">A <see cref="string"/> that contains the email subject.</param>
        /// <param name="body">A <see cref="string"/> that contains the message body. It can contain HTML content.</param>
        /// <param name="fromEmailAddress">A <see cref="string"/> that contains a valid email address of the sender.</param>
        /// <param name="recipients">A <see cref="List{T}"/> of  <see cref="string"/> that contains list of email addresses of one or more recipients.</param>
        /// <param name="ccList">A <see cref="List{T}"/> of <see cref="string"/> that contains list of email addresses of one or more CC recipients. This is optional.</param>
        /// <param name="attachments">A <see cref="List{T}"/> of <see cref="Services.EmailServiceProvider.Interface.FileAttachment"/> attachments. This is optional.</param>
        /// <returns>Returns <see langword="true"/> if the email was sent successfully else <see langword="false"/>.</returns>
        bool SendEmail(string subject, string body, string fromEmailAddress, IList<string> recipients, IList<string> ccList, IList<FileAttachment> attachments);
    }
}
