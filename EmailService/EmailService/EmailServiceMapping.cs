using AutoMapper;
using System;
using System.ServiceModel;

namespace Services.EmailService
{
    ///<summary>
    /// Email Service mapping profile. This class contains configuration to handle common mapping from one type to another.
    ///</summary> 
    public class EmailServiceMappingProfile : Profile
    {
        /// <summary>
        /// Name of the Mapping Profile.
        /// </summary>
        public override string ProfileName
        {
            get { return "EmailService"; }
        }

        /// <summary>
        /// The configuration of the actual mapper.
        /// This maps the exception look up implementation of the email service.
        /// </summary>
        protected override void Configure()
        {
            CreateMap<Exception, FaultException>().ConvertUsing(new ExceptionMapper());
        }

    }
}