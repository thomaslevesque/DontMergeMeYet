using System.Security.Cryptography;
using DontMergeMeYet.Services;

namespace DontMergeMeYet
{
    public class GithubSettings
    {
        public GithubSettings(string appId, string webhookSecret, string privateKey, string statusContext)
        {
            AppId = appId;
            WebhookSecret = webhookSecret;
            PrivateKey = privateKey;
            StatusContext = statusContext;
            RsaParameters =
                string.IsNullOrEmpty(privateKey)
                ? default
                : CryptoHelper.GetRsaParameters(privateKey);
        }

        public string AppId { get; }
        public string WebhookSecret { get; }
        public string PrivateKey { get; }
        public RSAParameters RsaParameters { get; }
        public string StatusContext { get; set; }
    }
}
