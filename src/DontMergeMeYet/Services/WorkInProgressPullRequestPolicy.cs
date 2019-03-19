using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Octokit;

namespace DontMergeMeYet.Services
{
    class WorkInProgressPullRequestPolicy : IPullRequestPolicy
    {
        public (CommitState state, string description) GetStatus(PullRequestContext context)
        {
            var pullRequest = context.PullRequestInfo;
            var containsWip = ContainsWip(context.RepositorySettings);

            if (containsWip(pullRequest.Title))
            {
                context.Logger.LogInformation("Pull request title matches WIP regex");
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.CommitMessages.Any(containsWip))
            {
                context.Logger.LogInformation("Commit message matches WIP regex");
                return (CommitState.Pending, "Work in progress");
            }

            var wipLabels = context.RepositorySettings.WipLabels ?? Array.Empty<string>();
            if (pullRequest.Labels.Intersect(wipLabels, StringComparer.OrdinalIgnoreCase).Any())
            {
                context.Logger.LogInformation("Pull request has a WIP label");
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.CommitMessages.Any(ShouldBeSquashed))
            {
                context.Logger.LogInformation("Commit message has squash or fixup prefix");
                return (CommitState.Pending, "Needs to be squashed before merging");
            }

            return (CommitState.Success, "Ready to merge");
        }

        private Func<string, bool> ContainsWip(RepositorySettings settings)
        {
            var patterns = settings.WipKeywords ?? Array.Empty<Keyword>();
            return text => patterns.Any(p => IsMatch(p, text));
        }

        private bool IsMatch(Keyword keyword, string text)
        {
            var regex = new Regex($@"\b{keyword.Text}\b",
                keyword.CaseSensitive
                    ? RegexOptions.None
                    : RegexOptions.IgnoreCase);
            return regex.IsMatch(text);
        }

        private static readonly string[] SquashPrefixes =
        {
            "fixup!",
            "squash!"
        };

        private bool ShouldBeSquashed(string text)
        {
            return SquashPrefixes.Any(p => text.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}