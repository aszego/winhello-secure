
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderInvalidKeyException : Exception
    {
        public AuthProviderInvalidKeyException()
        {
        }

        public AuthProviderInvalidKeyException(string? message) : base(message)
        {
        }

        public AuthProviderInvalidKeyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}