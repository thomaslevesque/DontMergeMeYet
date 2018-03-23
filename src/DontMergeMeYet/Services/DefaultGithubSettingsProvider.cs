using System.Configuration;

namespace DontMergeMeYet.Services
{
    class DefaultGithubSettingsProvider : IGithubSettingsProvider
    {
        public GithubSettings Settings { get; } = LoadSettings();

        private static GithubSettings LoadSettings()
        {
            return new GithubSettings(
                ConfigurationManager.AppSettings["GithubAppId"],
                ConfigurationManager.AppSettings["GithubWebhookSecret"],
                ConfigurationManager.AppSettings["GithubPrivateKey"],
                ConfigurationManager.AppSettings["GithubStatusContext"]);
        }
    }
}