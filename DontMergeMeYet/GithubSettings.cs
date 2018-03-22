using System.Security.Cryptography;
using DontMergeMeYet.Services;

namespace DontMergeMeYet
{
    public class GithubSettings
    {
        public string AppId { get; set; }
        public string WebhookSecret { get; set; }
        public string PrivateKey { get; set; }

        private RSAParameters? _rsaParameters;
        public RSAParameters RsaParameters
        {
            get
            {
                if (_rsaParameters == null && !string.IsNullOrEmpty(PrivateKey))
                {
                    _rsaParameters = CryptoHelper.GetRsaParameters(PrivateKey);
                }
                return _rsaParameters ?? default;
            }
        }
    }
}
