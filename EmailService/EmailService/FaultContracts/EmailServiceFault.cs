using System.Runtime.Serialization;

namespace ADP.DS.ServiceEdge.Services.EmailService.FaultContracts
{
    /// <summary>
    /// Faults that are specific to email service failures.
    /// </summary>
    [DataContract(Namespace = SchemaNamespaces.EmailServiceFault)]
    public enum EmailServiceFault
    {
        /// <summary>
        /// Returned when the input <see cref="ADP.DS.ServiceEdge.Services.EmailService.MessageContracts.SendEmailRequest"/> has one or more validation failures.
        /// </summary>
        [EnumMember]
        ValidationFailed
    }
}