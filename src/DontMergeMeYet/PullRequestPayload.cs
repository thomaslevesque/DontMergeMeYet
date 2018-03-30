using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestPayload : PullRequestEventPayload
    {
        public Installation Installation { get; set; }
    }
}