// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

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