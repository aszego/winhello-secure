
namespace WhSecure.AuthProviders
{
    [Serializable]
    internal class AuthProviderSystemErrorException : Exception
    {
        private string name;
        private int secStatus;

        public int ErrorCode => secStatus;

        public AuthProviderSystemErrorException()
        {
        }

        public AuthProviderSystemErrorException(string? message) : base(message)
        {
        }

        public AuthProviderSystemErrorException(string name, int secStatus)
        {
            this.name = name;
            this.secStatus = secStatus;
        }

        public AuthProviderSystemErrorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}