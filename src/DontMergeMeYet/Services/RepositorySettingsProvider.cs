using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DontMergeMeYet.Services
{
    class RepositorySettingsProvider : IRepositorySettingsProvider
    {
        private static readonly RepositorySettings DefaultSettings = new RepositorySettings
        {
            WipLabels = new[]
            {
                "wip",
                "work-in-progress",
                "in-progress"
            },
            WipKeywords = new[]
            {
                new Keyword { Text = "DO NOT MERGE", CaseSensitive = false },
                new Keyword { Text = "WIP", CaseSensitive = false }
            }
        };

        private static readonly Deserializer YamlDeserializer = CreateYamlDeserializer();

        private static Deserializer CreateYamlDeserializer()
        {
            var builder = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention());
            return builder.Build();
        }

        public async Task<RepositorySettings> GetRepositorySettingsAsync(PullRequestContext context)
        {
            try
            {
                var baseRef = context.Payload.PullRequest.Base.Ref;
                var repositoryId = context.Payload.Repository.Id;
                var client = new GitHubClient(context.GithubConnection);
                var contents = await client.Repository.Content.GetAllContentsByRef(repositoryId, "dontmergemeyet.yml", baseRef);
                var yaml = contents.FirstOrDefault()?.Content;
                var settings = !string.IsNullOrEmpty(yaml)
                    ? YamlDeserializer.Deserialize<RepositorySettings>(yaml)
                    : DefaultSettings;
                return settings;
            }
            catch (NotFoundException)
            {
                context.Log.Info("Configuration file not found, using defaults");
                return DefaultSettings;
            }
            catch(Exception ex)
            {
                context.Log.Error("Failed to get configuration file from repo, using defaults", ex);
                return DefaultSettings;
            }
        }
    }
}