using AutoMapper;

namespace ADP.DS.ServiceEdge.Services.EmailService.App_Code
{
    /// <summary>
    /// Initialization code for the application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Initializes automapper.
        /// </summary>
        public static void AppInitialize()
        {
            Mapper.Reset();
            Mapper.AddProfile<EmailServiceMappingProfile>();
            
        }
    }
}