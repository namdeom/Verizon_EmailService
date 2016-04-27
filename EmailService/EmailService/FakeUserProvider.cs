using System.Diagnostics.CodeAnalysis;
using Services.CommonLibraries.UserProvider.Interface;

namespace ADP.DS.ServiceEdge.Services.EmailService
{
    /// <summary>
    /// An implementation of the <see cref="Services.CommonLibraries.UserProvider.Interface.IUserProvider"/> used by email service.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FakeUserProvider : IUserProvider
    {
        /// <summary>
        /// Initializes the user provider.
        /// </summary>
        public void Initialize()
        {
            return;
        }

        /// <summary>
        /// Returns the <see cref="Services.CommonLibraries.UserProvider.Interface.User"/> corresponding to the validated <paramref name="sessionId"/>.
        /// If its not a valid session it returns <see langword="null"/>
        /// </summary>
        /// <param name="sessionId">The session token to be validated</param>
        /// <returns><see cref="Services.CommonLibraries.UserProvider.Interface.User"/> corresponding to the authentoicated <paramref name="sessionId"/>.</returns>
        public User GetUserFromSessionId(string sessionId, int storeId)
        {
            return null;
        }
    }
}