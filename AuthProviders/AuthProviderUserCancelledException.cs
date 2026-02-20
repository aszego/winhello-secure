// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

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