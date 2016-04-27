using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.Practices.Unity;

namespace ADP.DS.ServiceEdge.Services.EmailService.SmtpEmailServiceProvider
{
    /// <summary>
    /// The implementation of the <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.IEmailServiceProvider"/> that sends email by sending email
    /// through <b>SMTP</b>.
    /// </summary>
    public class SmtpEmailService : IEmailServiceProvider
    {

        /// <summary>
        /// An instance of <see cref="ILog"/> to be used for logging.
        /// </summary>
        private readonly ILog _logger;


        /// <summary>
        /// The class that implements the <see cref="ISmtpEmailService"/> for sending emails through <b>SMTP</b>.
        /// </summary>
        private ISmtpEmailService _smtpEmailService;

        
        /// <summary>
        /// The <b>SMTP</b> server IP address.
        /// </summary>
        private static string _smtpServer = ConfigurationManager.AppSettings["SmtpServer"];

        /// <summary>
        /// The <b>SMTP</b> server port.
        /// </summary>
        private static string _smtpPort = ConfigurationManager.AppSettings["SmtpPort"];

        /// <summary>
        /// Initializes a new instance of <see cref="SmtpEmailService"/>.
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to be used for logging.</param>
        [InjectionConstructor]
        public SmtpEmailService(ILog logger)
        {
            int port;
            Int32.TryParse(_smtpPort, out port);
            _logger = logger;
            _smtpEmailService = new DefaultSmtpEmailService(port,_smtpServer);
        }


        /// <summary>
        /// Initializes a new instance of <see cref="SmtpEmailService"/>.
        /// </summary>
        /// <param name="logger">An instance of <see cref="ILog"/> to be used for logging.</param>
        /// <param name="smtpEmailService">An instance of <see cref="ISmtpEmailService"/> for sending emails through <b>SMTP</b>.</param>
        public SmtpEmailService(ILog logger, ISmtpEmailService smtpEmailService)
        {
            _logger = logger;
            _smtpEmailService = smtpEmailService;
        }

        /// <summary>
        /// Sends an email using <b>SMTP</b>.
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
        /// <param name="attachments">A <see cref="List{T}"/> of <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/> attachments. This is optional.</param>
        /// <returns>Returns <see langword="true"/> if the email was sent successfully else <see langword="false"/>.</returns>
        public bool SendEmail(string subject, string body, string fromEmailAddress, IList<string> recipients, IList<string> ccList,
            IList<FileAttachment> attachments)
        {
            try
            {
                if (ValidateEmailParams(subject, body, fromEmailAddress, recipients, ccList, attachments))
                    return SendSmtpEmail(subject, body, fromEmailAddress, recipients, ccList, attachments);
                _logger.Error("[SendEmail] Invalid parameters. Email not sent.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.FatalFormat("[SendEmail] Unexpected exception. Message:{0}. StackTrace:{1}", ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Sends an email using <b>SMTP</b>.
        /// </summary>
        /// <param name="subject">A <see cref="string"/> that contains the email subject.</param>
        /// <param name="body">A <see cref="string"/> that contains the message body. It can contain HTML content.</param>
        /// <param name="fromEmailAddress">A <see cref="string"/> that contains a valid email address of the sender.</param>
        /// <param name="recipients">A <see cref="List{T}"/> of  <see cref="string"/> that contains list of email addresses of one or more recipients.</param>
        /// <param name="ccList">A <see cref="List{T}"/> of <see cref="string"/> that contains list of email addresses of one or more CC recipients. This is optional.</param>
        /// <param name="attachments">A <see cref="List{T}"/> of <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/> attachments. This is optional.</param>
        /// <returns>Returns <see langword="true"/> if the email was sent successfully else <see langword="false"/>.</returns>
        private bool SendSmtpEmail(string subject, string body, string fromEmailAddress, IList<string> recipients, IList<string> ccList, IList<FileAttachment> attachments)
        {
            //Initialize
            var isHtml =  body.IndexOf("<HTML>", StringComparison.InvariantCultureIgnoreCase) >= 0;
            if(ccList == null) ccList = new List<string>();
            if (attachments == null) attachments = new List<FileAttachment>();

            using (var message = new MailMessage())
            {
                try
                {
                message.From = GetValidMailAddress(fromEmailAddress);
              
                message.IsBodyHtml = isHtml;
                message.Body = body;
                message.Subject = subject;
                
                foreach (var email in recipients)
                {
                   message.To.Add(GetValidMailAddress(email));
                }

                foreach (var emailCc in ccList)
                {
                    message.CC.Add(GetValidMailAddress(emailCc));
                }
                    
                foreach (var attachment in from fileAttachment in attachments let bytes = Convert.FromBase64String(fileAttachment.Content) let memAttachment = new MemoryStream(bytes) select new Attachment(memAttachment, fileAttachment.Name))
                {
                    message.Attachments.Add(attachment);
                }
               
                    _smtpEmailService.Send(message);
                    _logger.Info("[SendSmtpEmail] Email sent successfully.");
                }
                catch(Exception ex)
                {
                    _logger.FatalFormat("[SendSmtpEmail] Email Failed. Message:{0}. StackTrace:{1}", ex.Message, ex.StackTrace);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Validates the input parameters for seding an email.
        /// </summary>
        /// <param name="subject">A <see cref="string"/> that contains the email subject.</param>
        /// <param name="body">A <see cref="string"/> that contains the message body. It can contain HTML content.</param>
        /// <param name="fromEmailAddress">A <see cref="string"/> that contains a valid email address of the sender.</param>
        /// <param name="recipients">A <see cref="List{T}"/> of  <see cref="string"/> that contains list of email addresses of one or more recipients.</param>
        /// <param name="ccList">A <see cref="List{T}"/> of <see cref="string"/> that contains list of email addresses of one or more CC recipients. This is optional.</param>
        /// <param name="attachments">A <see cref="List{T}"/> of <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/> attachments. This is optional.</param>
        /// <returns>Returns <see langword="true"/> if the parameters are valid else <see langword="false"/>.</returns>
        private bool ValidateEmailParams(string subject, string body, string fromEmailAddress, IList<string> recipients, IList<string> ccList, IList<FileAttachment> attachments)
        {
            _logger.Info("[ValidateEmailParams] validating input parameters.");
            if(subject != null) subject = subject.Trim();
            if (body != null) body = body.Trim();
            if (fromEmailAddress != null) fromEmailAddress = fromEmailAddress.Trim();
          
            if (string.IsNullOrEmpty(subject))
            {
                _logger.Error("The subject is empty.");
                return false;
            }

            if (string.IsNullOrEmpty(body))
            {
                _logger.Error("The email body content is empty.");
                return false;
            }

            if (string.IsNullOrEmpty(fromEmailAddress))
            {
                _logger.Error("The from email address is empty.");
                return false;
            }

            if (!IsValidEmailAddress(fromEmailAddress))
            {
                _logger.ErrorFormat("The from email address [{0}] is invalid.",fromEmailAddress);
                return false;
            }

            if (recipients == null || !recipients.Any())
            {
                _logger.Error("The recipients list is empty.");
                return false;
            }

            foreach (var email in recipients.Where(email => !IsValidEmailAddress(email)))
            {
                _logger.ErrorFormat("The recipient email address [{0}] is invalid.", email);
                return false;
            }

            if (attachments != null && attachments.Any())
            {
                if (attachments.Any(attachment => string.IsNullOrEmpty(attachment.Content) || string.IsNullOrEmpty(attachment.Name)))
                {
                    _logger.Error("The attachment is invalid.");
                    return false;
                }
                foreach (var attachment in attachments)
                {
                    if(attachment.Content != null)
                    attachment.Content = attachment.Content.Trim();
                    if (attachment.Name != null)
                        attachment.Name = attachment.Name.Trim();
                    if(string.IsNullOrEmpty(attachment.Content) || string.IsNullOrEmpty(attachment.Name)) return false;
                }
            }

            if (ccList == null || !ccList.Any()) return true;
            foreach (var email in ccList.Where(email => !IsValidEmailAddress(email)))
            {
                _logger.ErrorFormat("The cclist email address [{0}] is invalid.", email);
                return false;
            }

            //after this no check require because even it is null we can proceed.CC member is not mandatory.

            _logger.Info("Valid email parameters.");
            return true;
        }

        
        

        /// <summary>
        /// Validates if the <paramref name="emailaddress"/> is a valid email address.
        /// </summary>
        /// <param name="emailaddress">A <see cref="string"/> that contains the email address to be validated.</param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="emailaddress"/> is a valid email address else <see langword="false"/>.</returns>
        private bool IsValidEmailAddress(string emailaddress)
        {
            if (emailaddress.LastIndexOf("<", System.StringComparison.InvariantCultureIgnoreCase) >= 0 && emailaddress.LastIndexOf(">", System.StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                emailaddress = emailaddress.Split(new char[] { '<', '>' })[1];
            }
            if (!IsValidEmail(emailaddress))
            {
                _logger.ErrorFormat("[IsValidEmailAddress]  RegEx validation failed for [{0}]. Invalid Email address", emailaddress);
                return false;
            }
            var email = new MailAddress(emailaddress);
            return !string.IsNullOrEmpty(email.Address);
        }

        /// <summary>
        /// Validates if the <paramref name="emailaddress"/> is a valid email address.
        /// </summary>
        /// <param name="emailaddress">A <see cref="string"/> that contains the email address to be validated.</param>
        /// <returns>Returns <see langword="true"/> if the <paramref name="emailaddress"/> is a valid email address else <see langword="false"/>.</returns>
        private static bool IsValidEmail(string emailaddress)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(emailaddress, 
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        private static MailAddress GetValidMailAddress(string emailAddress)
        {
            if (emailAddress.LastIndexOf("<", System.StringComparison.InvariantCultureIgnoreCase) < 0 ||
                emailAddress.LastIndexOf(">", System.StringComparison.InvariantCultureIgnoreCase) < 0)
                return new MailAddress(emailAddress);
            var index = emailAddress.LastIndexOf("<", System.StringComparison.InvariantCultureIgnoreCase);
            return new MailAddress(emailAddress.Substring(index + 1).Replace(">", string.Empty), emailAddress.Substring(0, index));
        }
    }

    /// <summary>
    /// This interface is used to abstract the actual implementation away from the smtp email service. 
    /// </summary>
    public interface ISmtpEmailService
    {
        void Send(MailMessage message);
    }

    /// <summary>
    /// This is the default class that implements the <see cref="ISmtpEmailService"/>
    /// that sends out emails using <b>SMTP</b>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DefaultSmtpEmailService : ISmtpEmailService
    {
        private SmtpClient _smtpClient;
        /// <summary>
        /// Initializes the smttp client to send email.
        /// </summary>
        /// <param name="smtpPort">The <b>SMTP</b> port.</param>
        /// <param name="smtpServer">The <b>SMTP</b> server.</param>
        public DefaultSmtpEmailService(int smtpPort, string smtpServer)
        {
            _smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true
            };
        }
        /// <summary>
        /// Sends the email using <b>SMTP</b>
        /// </summary>
        /// <param name="message">The <see cref="MailMessage"/> that contains the email message to be sent</param>
        public void Send(MailMessage message)
        {
           _smtpClient.Send(message);
        }
    }
}
