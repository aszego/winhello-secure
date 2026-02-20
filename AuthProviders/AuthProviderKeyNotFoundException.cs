
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderKeyNotFoundException : Exception
    {
        public AuthProviderKeyNotFoundException()
        {
        }

        public AuthProviderKeyNotFoundException(string? message) : base(message)
        {
        }

        public AuthProviderKeyNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}