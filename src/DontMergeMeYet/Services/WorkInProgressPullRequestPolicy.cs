using System;
using System.Linq;
using System.Text.RegularExpressions;
using Octokit;

namespace DontMergeMeYet.Services
{
    class WorkInProgressPullRequestPolicy : IPullRequestPolicy
    {
        public (CommitState state, string description) GetStatus(
            PullRequestContext context,
            PullRequestInfo pullRequest)
        {
            if (ContainsWip(pullRequest.Title))
            {
                context.Log.Info("Pull request title matches WIP regex");
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.CommitMessages.Any(ContainsWip))
            {
                context.Log.Info("Commit message matches WIP regex");
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.Labels.Any(ContainsWip))
            {
                context.Log.Info("Label matches WIP regex");
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.CommitMessages.Any(ShouldBeSquashed))
            {
                context.Log.Info("Commit message has squash or fixup prefix");
                return (CommitState.Pending, "Needs to be squashed before merging");
            }

            return (CommitState.Success, "Ready to merge");
        }

        private static readonly Regex[] WipRegexes =
        {
            new Regex(@"\bwip\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase),
            new Regex("\bdo not merge\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
        };

        private bool ContainsWip(string text)
        {
            return WipRegexes.Any(r => r.IsMatch(text));
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