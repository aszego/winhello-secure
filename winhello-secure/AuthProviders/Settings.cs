// Derived from KeePassWinHello (MIT) - https://github.com/sirAndros/KeePassWinHello
// winhello-secure - part of winhello-secure (GPL-3.0)

namespace WhSecure.AuthProviders
{
    internal class Settings
    {
        internal const string ProductName = "WhSecure";

        public static int MAX_RETRY_COUNT { get; internal set; }
        public static int ATTEMPT_DELAY { get; internal set; }
    }
}