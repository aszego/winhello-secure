
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderException : Exception
    {
        public AuthProviderException()
        {
        }

        public AuthProviderException(string? message) : base(message)
        {
        }

        public AuthProviderException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}