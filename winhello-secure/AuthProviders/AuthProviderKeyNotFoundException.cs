// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

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