using System.Text.RegularExpressions;
using Services.CommonLibraries.Infrastructure;
using Services.CommonLibraries.Infrastructure.Exceptions;
using Services.CommonLibraries.UserProvider.Interface;
using ADP.DS.ServiceEdge.Services.EmailService.FaultContracts;
using ADP.DS.ServiceEdge.Services.EmailService.MessageContracts;
using ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts;
using ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface;
using AutoMapper;
using Common.Logging;
using System;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Collections.Generic;
namespace ADP.DS.ServiceEdge.Services.EmailService
{
    /// <summary>
    /// Used to send email. Implements the <see cref="ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts.IEmailService"/>.
    /// </summary>
    /// <seealso cref="ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts.IEmailService"/>
    public class EmailService : WcfServiceBase, IEmailService
    {
        private readonly IEmailServiceProvider _emailServiceProvider;
        private readonly IBadEmailDomainsCheckProvider _badDomainCheckProvider;
        private static string _sessionToken = System.Configuration.ConfigurationManager.AppSettings["EmailServiceToken"];
        private const string ValidationMessage = "Validation failed. Please see the logs.";


        /// <summary>
        /// Constructor that intializes the default logging and other providers.
        /// </summary>
        /// <param name="logger">The <see cref="ILog"/> for logging.</param>
        /// <param name="mapEngine">The <see cref="IMappingEngine"/> for mapping.</param>
        /// <param name="emailServiceProvider">The actual implementation of the <see cref="IEmailServiceProvider"/>.</param>
        /// <param name="userProvider">The actual implementation of the <see cref="IUserProvider"/>.</param>
        /// <param name="badDomainCheckProvider"></param>
        public EmailService(ILog logger, IMappingEngine mapEngine, IEmailServiceProvider emailServiceProvider, IBadEmailDomainsCheckProvider badDomainCheckProvider, IUserProvider userProvider)
            : base(logger, mapEngine, userProvider)
        {
            _emailServiceProvider = emailServiceProvider;
            _badDomainCheckProvider = badDomainCheckProvider;
        }

        /// <summary>
        /// The implementation of the <see cref="ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts.IEmailService.SendEmail"/>.
        /// It validates the input request and sends an email.
        /// </summary>
        /// <param name="request">The <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> that contains the input request for sending an email to one or more recipients with one or more <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/>.</param>
        /// <returns>A <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailResponse"/> that contains the information if the email was sent successfully.</returns>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws <see cref="Services.CommonLibraries.Infrastructure.Faults.SystemFault"/> when any unhandled exception occours.
        /// </exception>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws this fault <see cref="ADP.DS.ServiceEdge.Services.EmailService.FaultContracts.EmailServiceFault.ValidationFailed"/> when the <paramref name="request"/> fails validation.
        /// Following are the possible validation failures.
        /// <list type="bullet">
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Subject"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Body"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.FromEmailAddress"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/> or is an invalid email address.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Recipients"/></term>
        /// <description>When it is <see langword="null"/> or empty or  contains one or more invalid email addresses.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.CarbonCopyList"/></term>
        /// <description>When it contains one or more invalid email addresses.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Attachments"/></term>
        /// <description>When it contains one or more invalid attachments that is not base 64 encoded or missing file name.</description>
        /// </item> 
        /// </list>
        /// </exception>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws <see cref="Services.CommonLibraries.Infrastructure.Faults.AuthorizationFault"/> when authorization fails.
        /// </exception>
        public SendEmailResponse SendEmail(SendEmailRequest request)
        {
            SendEmailResponse response = null;
            try
            {
                Logger.Debug("[SendEmail] Validating the sessionId.");
                if (string.Compare(request.SessionId, _sessionToken, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw new InvalidSessionIdException();
                }

                Logger.Debug("[SendEmail] Validating input parameters.");

                if (!ValidateEmailParams(request))
                {
                    Logger.Error("[SendEmail] Input request is invalid.");
                    throw new FaultException<EmailServiceFault>(EmailServiceFault.ValidationFailed, ValidationMessage);
                }

                Logger.Debug("[SendEmail] Sending the email.");

                var result = _emailServiceProvider.SendEmail(request.Subject, request.Body, request.FromEmailAddress,
                    request.Recipients, request.CarbonCopyList, request.Attachments);

                response = new SendEmailResponse { IsSuccess = result };
                if (response.IsSuccess)
                {
                    Logger.InfoFormat("[SendEmail] Email was sent successfully. Result :[{0}]", result);
                }
                else
                {
                    Logger.InfoFormat("[SendEmail] Email could not send .Please check logs for more info Result :[{0}]", result);
                }
            }
            catch (Exception ex)
            {
                Logger.FatalFormat("[SendEmail] Unexpected exception. Message:{0}. Stacktrace:{1}", ex, ex.Message, ex.StackTrace);
                throw GetFaultException(request, ex);
            }
            return response;
        }

        /// <summary>
        /// The implementation of the <see cref="ADP.DS.ServiceEdge.Services.EmailService.ServiceContracts.ISendEmailToValidAddressService.SendEmailWithBadAddressCheck"/>.
        /// It validates the input request and sends an email.
        /// </summary>
        /// <param name="request">The <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> that contains the input request for sending an email to one or more recipients with one or more <see cref="ADP.DS.ServiceEdge.Services.EmailServiceProvider.Interface.FileAttachment"/>.</param>
        /// <returns>A <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailResponseWithBadAddressCheck"/> that contains the information if the email was sent successfully along with list of invalid Recipients.</returns>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws <see cref="Services.CommonLibraries.Infrastructure.Faults.SystemFault"/> when any unhandled exception occours.
        /// </exception>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws this fault <see cref="ADP.DS.ServiceEdge.Services.EmailService.FaultContracts.EmailServiceFault.ValidationFailed"/> when the <paramref name="request"/> fails validation.
        /// Following are the possible validation failures.
        /// <list type="bullet">
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Subject"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Body"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/></description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.FromEmailAddress"/></term>
        /// <description>When it is <see langword="null"/> or <see cref="System.String.Empty"/> or is an invalid email address.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Recipients"/></term>
        /// <description>When it is <see langword="null"/> or empty or  contains one or more invalid email addresses.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.CarbonCopyList"/></term>
        /// <description>When it contains one or more invalid email addresses.</description>
        /// </item>
        /// <item>
        /// <term><see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest.Attachments"/></term>
        /// <description>When it contains one or more invalid attachments that is not base 64 encoded or missing file name.</description>
        /// </item> 
        /// </list>
        /// </exception>
        /// <exception cref="System.ServiceModel.FaultException">
        /// Throws <see cref="Services.CommonLibraries.Infrastructure.Faults.AuthorizationFault"/> when authorization fails.
        /// </exception>
        public SendEmailWithBadAddressCheckResponse SendEmailWithBadAddressCheck(SendEmailRequest request)
        {
            SendEmailWithBadAddressCheckResponse response = null;
            bool result = false;
            try
            {
                Logger.Debug("[SendEmailWithBadAddressCheck] Validating the sessionId.");
                if (string.Compare(request.SessionId, _sessionToken, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    throw new InvalidSessionIdException();
                }

                Logger.Debug("[SendEmailWithBadAddressCheck] Validating input parameters.");

                if (!ValidateEmailParameters(request))
                {
                    Logger.Error("[SendEmailWithBadAddressCheck] Input request is invalid.");
                    throw new FaultException<EmailServiceFault>(EmailServiceFault.ValidationFailed, ValidationMessage);
                }

                Logger.Debug("[SendEmailWithBadAddressCheck] Sending the email.");
                InvalidRecipients InvalidRecipients = ValidateEmailAddress(request);

                //removal of Recipients from the list who have bad domains in address and mail is getting triggered to recepeints with valid domain address
                request.Recipients.RemoveAll(x => InvalidRecipients.BadDomains.Contains(x));
                if (request.CarbonCopyList != null)
                    request.CarbonCopyList.RemoveAll(x => InvalidRecipients.BadDomains.Contains(x));


                if (request.Recipients != null && request.Recipients.Any())
                {

                    result = _emailServiceProvider.SendEmail(request.Subject, request.Body, request.FromEmailAddress,
                    request.Recipients, request.CarbonCopyList, request.Attachments);
                }
                if (result)
                {
                    Logger.InfoFormat("[SendEmailWithBadAddressCheck] Email was sent successfully. Result :[{0}]", result);
                }
                else
                {
                    Logger.InfoFormat("[SendEmailWithBadAddressCheck] Email could not send .Please check logs for more info Result :[{0}]", result);
                }
                //List of bad domain and bad address is added to response with IsSuccess flag
                response = new SendEmailWithBadAddressCheckResponse { IsSuccess = result, InvalidRecipients = InvalidRecipients };

            }
            catch (Exception ex)
            {
                Logger.FatalFormat("[SendEmailWithBadAddressCheck] Unexpected exception. Message:{0}. Stacktrace:{1}", ex, ex.Message, ex.StackTrace);
                throw GetFaultException(request, ex);
            }
            return response;
        }

        /// <summary>
        /// Validates the input email request.
        /// </summary>
        /// <param name="request">The <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> to be validated.</param>
        /// <returns><see langword="true"/> if <paramref name="request"/> is a valid request else <see langword="false"/>.</returns>
        private bool ValidateEmailParams(SendEmailRequest request)
        {
            if (!ValidateEmailParameters(request))
            {
                return false;
            }


            //Validate domain
            foreach (var email in request.Recipients.Where(email => _badDomainCheckProvider.IsBadDomain(email)))
            {
                Logger.ErrorFormat("The recipient email address [{0}] is not a supported domain.", email);
                return false;
            }
            if (request.CarbonCopyList != null && request.CarbonCopyList.Any())
            {
                //validate bad domain
                foreach (var email in request.CarbonCopyList.Where(email => _badDomainCheckProvider.IsBadDomain(email)))
                {
                    Logger.ErrorFormat("The cclist email address [{0}] is not a supported domain.", email);
                    return false;
                }
            }
            Logger.Info("Valid email parameters.");
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
                Logger.ErrorFormat("[IsValidEmailAddress]  RegEx validation failed for [{0}]. Invalid Email address", emailaddress);
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
            return Regex.IsMatch(emailaddress,
                     @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                     @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                     RegexOptions.IgnoreCase);

        }

        /// <summary>
        /// Validates the input email request.
        /// </summary>
        /// <param name="request">The <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> to be validated.</param>
        /// <returns><see langword="true"/> if <paramref name="request"/> is a valid request else <see langword="false"/>.</returns>
        private bool ValidateEmailParameters(SendEmailRequest request)
        {
            Logger.Info("[ValidateEmailParameters] validating input parameters.");
            Logger.Info(request.ToString());
            if (request.Subject != null) request.Subject = request.Subject.Trim();
            if (request.Body != null) request.Body = request.Body.Trim();
            if (request.FromEmailAddress != null) request.FromEmailAddress = request.FromEmailAddress.Trim();

            if (string.IsNullOrEmpty(request.Subject))
            {
                Logger.Error("The subject is empty.");
                return false;
            }

            if (string.IsNullOrEmpty(request.Body))
            {
                Logger.Error("The email body content is empty.");
                return false;
            }

            if (string.IsNullOrEmpty(request.FromEmailAddress))
            {
                Logger.Error("The from email address is empty.");
                return false;
            }

            if (request.Recipients == null || !request.Recipients.Any())
            {
                Logger.Error("The recipients list is empty.");
                return false;
            }

            if (!IsValidEmailAddress(request.FromEmailAddress))
            {
                Logger.ErrorFormat("The from email address [{0}] is invalid.", request.FromEmailAddress);
                return false;
            }

            foreach (var email in request.Recipients.Where(email => !IsValidEmailAddress(email)))
            {
                Logger.ErrorFormat("The recipient email address [{0}] is invalid.", email);
                return false;
            }

            if (request.CarbonCopyList != null && request.CarbonCopyList.Any())
            {
                foreach (var email in request.CarbonCopyList.Where(email => !IsValidEmailAddress(email)))
                {
                    Logger.ErrorFormat("The cclist email address [{0}] is invalid.", email);
                    return false;
                }
            }

            var attachments = request.Attachments;
            var invalidAttachments = false;
            if (attachments != null && attachments.Any())
            {
                if (attachments.Any(attachment => string.IsNullOrEmpty(attachment.Content) || string.IsNullOrEmpty(attachment.Name)))
                {
                    Logger.Error("The attachment is invalid.");
                    return false;
                }
                foreach (var attachment in attachments)
                {
                    if (attachment.Content != null)
                        attachment.Content = attachment.Content.Trim();
                    if (attachment.Name != null)
                        attachment.Name = attachment.Name.Trim();
                    if (!string.IsNullOrEmpty(attachment.Content) && !string.IsNullOrEmpty(attachment.Name)) continue;
                    invalidAttachments = true;
                    break;
                }
            }

            if (invalidAttachments)
            {
                Logger.Error("The attachment is invalid.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the input email request.
        /// </summary>
        /// <param name="request">The <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> to be validated.</param>
        /// <returns>A <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.InvalidRecipients"/> that contains the information if Recipients address is invalid or with bad domain.</returns>
        private InvalidRecipients ValidateEmailAddress(SendEmailRequest request)
        {
            InvalidRecipients resplist = new InvalidRecipients();
            Logger.Info("[ValidateEmailAddress] validating Recipients email addresses.");
            Logger.Info(request.ToString());

            //Validate domain
            foreach (var email in request.Recipients.Where(email => _badDomainCheckProvider.IsBadDomain(email)))
            {
                Logger.ErrorFormat("The recipient email address [{0}] is not a supported domain.", email);
                resplist.BadDomains.Add(email);
            }
            if (request.CarbonCopyList == null) return resplist;

            //validate bad domain
            foreach (var email in request.CarbonCopyList.Where(email => _badDomainCheckProvider.IsBadDomain(email)))
            {
                Logger.ErrorFormat("The cclist email address [{0}] is not a supported domain.", email);
                resplist.BadDomains.Add(email);
            }
            return resplist;

        }
    }
}
