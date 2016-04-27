using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EmailTestApp.EmailServiceAddressCheck;

namespace EmailTestApp
{
    public partial class EmailTestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack) return;
            this.txtBody.Text = string.Empty;
            this.txtCC.Text = string.Empty;
            this.txtFromEmail.Text = string.Empty;
            this.txtTo.Text = string.Empty;
            this.txtSubject.Text = string.Empty;
            this.errorText.Text = string.Empty;
            this.errorText.Visible = false;
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                this.btnSend.Enabled = false;
                this.errorText.Visible = true;
                return;
            }
            this.errorText.Text = string.Empty;
            this.errorText.Visible = false;
            SendEmail(btnSend.CommandName);
        }

        private void SendEmail(string btncommand)
        {
            try
            {
                var attachments = new List<FileAttachment>();
                if (fileAttachment.HasFile)
                {
                    var file = new FileAttachment
                    {
                        Name = fileAttachment.FileName,
                        Content = Convert.ToBase64String(fileAttachment.FileBytes)
                    };
                     attachments.Add(file);
                }
               
               
                var request = new SendEmailRequest
                {
                    Body = this.txtBody.Text,
                    FromEmailAddress = String.IsNullOrEmpty(this.txtFromEmail.Text) ? null : this.txtFromEmail.Text,
                    Recipients = String.IsNullOrEmpty(this.txtTo.Text) ? null : this.txtTo.Text.Split(';'),
                    CarbonCopyList = String.IsNullOrEmpty(this.txtCC.Text) ? null : this.txtCC.Text.Split(';'),
                    SessionId = "6C06251C-377E-4803-A96F-5CC10490748B",
                    Subject = this.txtSubject.Text,
                    Attachments = attachments.Count > 0 ? attachments.ToArray() : null

                };
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                 ((sender, certificate, chain, sslPolicyErrors) => true);
               
                if(btncommand=="Sendmail")
                using (var proxy = new EmailServiceClient())
                {
                    var response = proxy.SendEmail(request);
                    if (response != null && response.IsSuccess)
                    {
                        this.errorText.Visible = true;
                        this.errorText.Text = "Email succeeded.";
                        return;
                    }
                    this.errorText.Visible = true;
                    this.errorText.Text = "Email failed.";
                }

                else
                    using (var proxy = new EmailServiceClient())
                {
                    var response = proxy.SendEmailWithBadAddressCheck(request);
                    if (response != null && response.IsSuccess)
                    {
                        this.errorText.Visible = true;
                        this.errorText.Text = "Email succeeded.";

                    }
                    else
                    {
                        this.errorText.Visible = true;
                        this.errorText.Text = "Email failed.";
                    }
                        if (response.InvalidRecipients.BadDomains.Count() != 0)
                        this.errorText.Text += String.Format("\t Email address with bad domains list:-\t {0}", String.Join(",",response.InvalidRecipients.BadDomains));
                }
            }
            catch (FaultException<EmailServiceFault> fxException)
            {
                this.errorText.Visible = true;
                this.errorText.Text = fxException.Detail.ToString();
            }
            catch (Exception ex)
            {
                this.errorText.Visible = true;
                this.errorText.Text = ex.Message + ex.StackTrace;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(this.txtFromEmail.Text))
            {
                this.errorText.Text = "From email address is required";
                return false;
            }
            if (string.IsNullOrEmpty(this.txtTo.Text))
            {
                this.errorText.Text = "To email address is required";
                return false;
            }
            if (string.IsNullOrEmpty(this.txtSubject.Text))
            {
                this.errorText.Text = "Subject is required";
                return false;
            }
            if (!string.IsNullOrEmpty(this.txtBody.Text)) return true;
            this.errorText.Text = "Email body is required";
            return false;
        }

        static public string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            var returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        protected void btnSend0_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                this.btnSend.Enabled = false;
                this.errorText.Visible = true;
                return;
            }
            this.errorText.Text = string.Empty;
            this.errorText.Visible = false;
            SendEmail(btnSend0.CommandName);
        }

    }
}