using Services.CommonLibraries.Infrastructure;
using Services.EmailServiceProvider.Interface;
using System.Collections.Generic;
using System.ServiceModel;

namespace Services.EmailService.MessageContracts
{
    /// <summary>
    /// The input request for the <see cref="Services.EmailService.ServiceContracts.IEmailService.SendEmail"/> operation.
    /// </summary>
    [MessageContract(WrapperNamespace = SchemaNamespaces.EmailServiceMessage)]
    public class SendEmailRequest : RequestMessageBase
    {
        /// <summary>
        /// A <see cref="string"/> that contains the email subject. It is a required field.
        /// </summary>
        [MessageBodyMember]
        public string Subject { get; set; }

        /// <summary>
        /// A <see cref="string"/> that contains the email body. It is a required field. It can contain <b>HTML</b> content.
        /// </summary>
        [MessageBodyMember]
        public string Body { get; set; }

        /// <summary>
        /// A <see cref="string"/> that contains a valid email address of the sender. It is a required field.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value should contain valid SMTP email address in proper format as below:
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
        [MessageBodyMember]
        public string FromEmailAddress { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of  <see cref="string"/> that contains list of valid email addresses of one or more recipients. It is a required field.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The email address in the list should contain valid SMTP email address in proper format as below:
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
        [MessageBodyMember]
        public List<string> Recipients { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of  <see cref="string"/> that contains list of valid email addresses of one or more CC recipients. It is an optional field.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The email address in the list should contain valid SMTP email address in proper format as below:
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
        [MessageBodyMember]
        public List<string> CarbonCopyList { get; set; }

        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Services.EmailServiceProvider.Interface.FileAttachment"/> attachments that are sent as part of the email. This is optional.
        /// </summary>
        [MessageBodyMember]
        public List<FileAttachment> Attachments { get; set; }
    }
}