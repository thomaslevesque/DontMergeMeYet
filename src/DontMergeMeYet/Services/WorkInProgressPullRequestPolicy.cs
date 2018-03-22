using System;
using System.Linq;
using Octokit;

namespace DontMergeMeYet.Services
{
    class WorkInProgressPullRequestPolicy : IPullRequestPolicy
    {
        public bool IsReadyToMerge(PullRequestInfo pullRequest)
        {
            return !ContainsWip(pullRequest.Title)
                   && !pullRequest.CommitMessages.Any(m => ContainsWip(m) || ShouldBeSquashed(m));
        }

        private const string CommitStatusContext = "DontMergeMeYet";

        public NewCommitStatus GetStatus(PullRequestInfo pullRequest)
        {
           if (ContainsWip(pullRequest.Title) || pullRequest.CommitMessages.Any(ContainsWip))
            {
           
                return new NewCommitStatus
                {
                    Context = CommitStatusContext,
                    State = CommitState.Pending,
                    Description = "Work in progress"
                };
            }

            if (pullRequest.CommitMessages.Any(ShouldBeSquashed))
            {
                return new NewCommitStatus
                {
                    Context = CommitStatusContext,
                    State = CommitState.Pending,
                    Description = "Needs to be squashed before merging"
                };
            }

            return new NewCommitStatus
            {
                Context = CommitStatusContext,
                State = CommitState.Success,
                Description = "Ready to merge"
            };
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