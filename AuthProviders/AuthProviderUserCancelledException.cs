
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderUserCancelledException : Exception
    {
        public AuthProviderUserCancelledException()
        {
        }

        public AuthProviderUserCancelledException(string? message) : base(message)
        {
        }

        public AuthProviderUserCancelledException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}