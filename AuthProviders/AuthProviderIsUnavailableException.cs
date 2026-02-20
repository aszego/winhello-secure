
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderIsUnavailableException : Exception
    {
        public AuthProviderIsUnavailableException()
        {
        }

        public AuthProviderIsUnavailableException(string? message) : base(message)
        {
        }

        public AuthProviderIsUnavailableException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}