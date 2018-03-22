using System;
using System.Linq;
using Octokit;

namespace DontMergeMeYet.Services
{
    class WorkInProgressPullRequestPolicy : IPullRequestPolicy
    {
        public (CommitState state, string description) GetStatus(PullRequestInfo pullRequest)
        {
            if (ContainsWip(pullRequest.Title) ||
                pullRequest.CommitMessages.Any(ContainsWip) ||
                pullRequest.Labels.Any(ContainsWip))
            {
                return (CommitState.Pending, "Work in progress");
            }

            if (pullRequest.CommitMessages.Any(ShouldBeSquashed))
            {
                return (CommitState.Pending, "Needs to be squashed before merging");
            }

            return (CommitState.Success, "Ready to merge");
        }

        private static readonly string[] WipKeywords =
        {
            "wip",
            "do not merge"
        };

        private bool ContainsWip(string text)
        {
            return WipKeywords.Any(k => text.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0);
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