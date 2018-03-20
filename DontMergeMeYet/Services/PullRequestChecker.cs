using System;
using System.Linq;
using DontMergeMeYet.Models;
using DontMergeMeYet.Models.Github;

namespace DontMergeMeYet.Services
{
    class PullRequestChecker : IPullRequestChecker
    {
        public bool IsReadyToMerge(PullRequestInfo pullRequest)
        {
            return !ContainsWip(pullRequest.Title)
                   && !pullRequest.CommitMessages.Any(m => ContainsWip(m) || ShouldBeSquashed(m));
        }

        private const string CommitStatusContext = "DontMergeMeYet";

        public CommitStatus GetStatus(PullRequestInfo pullRequest)
        {
            if (ContainsWip(pullRequest.Title) || pullRequest.CommitMessages.Any(ContainsWip))
            {
                return new CommitStatus
                {
                    Context = CommitStatusContext,
                    State = CommitStatusState.Pending,
                    Description = "Work in progress"
                };
            }

            if (pullRequest.CommitMessages.Any(ShouldBeSquashed))
            {
                return new CommitStatus
                {
                    Context = CommitStatusContext,
                    State = CommitStatusState.Pending,
                    Description = "Needs to be squashed before merging"
                };
            }

            return new CommitStatus
            {
                Context = CommitStatusContext,
                State = CommitStatusState.Success,
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