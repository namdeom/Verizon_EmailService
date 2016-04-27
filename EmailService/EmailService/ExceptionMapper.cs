using Services.CommonLibraries.Infrastructure.Exceptions;
using Services.CommonLibraries.Infrastructure.Faults;
using ADP.DS.ServiceEdge.Services.EmailService.FaultContracts;
using AutoMapper;
using System;
using System.ServiceModel;

namespace ADP.DS.ServiceEdge.Services.EmailService
{
    /// <summary>
    ///   A class that maps all the exceptions to its corresponding fault.
    /// </summary>
    public class ExceptionMapper : TypeConverter<Exception, FaultException>
    {
        /// <summary>
        /// A <see cref="string"/> message that is displayed when a SystemFault occurs
        /// </summary>
        private const string SystemFaultMessage = "System Fault. Please see the logs.";

        /// <summary>
        ///  Returns the corresponding fault for the exception source.
        /// </summary>
        /// <param name="source"> Exception instance. </param>
        /// <returns> <see cref="System.ServiceModel.FaultException"/> that corresponds to the type of exception. </returns>
        protected override FaultException ConvertCore(Exception source)
        {
            FaultException returnFault;
            var exceptionType = source.GetType();
            var fault = source.GetBaseException() as FaultException<EmailServiceFault>;

            if (fault != null)
            {
                return new FaultException<EmailServiceFault>(EmailServiceFault.ValidationFailed, source.Message);
            }

            if (exceptionType == typeof(InvalidSessionIdException))
            {
                returnFault = new FaultException<AuthorizationFault>(new AuthorizationFault(), source.Message);
            }
            else
            {
                returnFault = new FaultException<SystemFault>(new SystemFault(), SystemFaultMessage);
            }
            return returnFault;
        }


    }
}