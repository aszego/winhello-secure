// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

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