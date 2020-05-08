using System;
using System.Linq;
using System.Threading.Tasks;
using DontMergeMeYet.Services.Abstractions;
using Microsoft.Extensions.Logging;
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

        private static readonly IDeserializer YamlDeserializer = CreateYamlDeserializer();

        private static IDeserializer CreateYamlDeserializer()
        {
            var builder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
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
                context.Logger.LogInformation("Configuration file not found, using defaults");
                return DefaultSettings;
            }
            catch(Exception ex)
            {
                context.Logger.LogError(ex, "Failed to get configuration file from repo, using defaults");
                return DefaultSettings;
            }
        }
    }
}