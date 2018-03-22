using System.Configuration;
using System.Security.Cryptography;
using DontMergeMeYet.Services;

namespace DontMergeMeYet
{
    public class GithubSettings
    {
        public GithubSettings(string appId, string webhookSecret, string privateKey)
        {
            AppId = appId;
            WebhookSecret = webhookSecret;
            PrivateKey = privateKey;
            RsaParameters = CryptoHelper.GetRsaParameters(privateKey);
        }

        public string AppId { get; }
        public string WebhookSecret { get; }
        public string PrivateKey { get; }
        public RSAParameters RsaParameters { get; }

        public static GithubSettings Default { get; } = LoadSettings();

        private static GithubSettings LoadSettings()
        {
            return new GithubSettings(
                ConfigurationManager.AppSettings["GithubAppId"],
                ConfigurationManager.AppSettings["GithubWebhookSecret"],
                ConfigurationManager.AppSettings["GithubPrivateKey"]);
        }
    }
}
