
namespace ADP.DS.ServiceEdge.Services.EmailService
{
    /// <summary>
    /// The email service endpoint namespaces.
    /// </summary>
    public class SchemaNamespaces
    {
        /// <summary>
        /// A <see cref="System.String"/> to represent the service contract namespace.
        /// </summary>
        public const string EmailService = "http://adp.com/ds/serviceedge/services/emailservice";

        /// <summary>
        /// A <see cref="System.String"/> to represent the data contract namespace.
        /// </summary>
        public const string EmailServiceData = EmailService + "/data";

        /// <summary>
        /// A <see cref="System.String"/> to represent the message contract namespace.
        /// </summary>
        public const string EmailServiceMessage = EmailService + "/message";

        /// <summary>
        /// A <see cref="System.String"/> to represent the fault contract namespace.
        /// </summary>
        public const string EmailServiceFault = EmailService + "/fault";
    }
}