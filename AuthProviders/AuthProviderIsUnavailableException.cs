// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

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