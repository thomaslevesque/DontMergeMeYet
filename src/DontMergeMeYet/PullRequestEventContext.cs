using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestEventContext
    {
        public PullRequestPayload Payload { get; }
        public IConnection GithubConnection { get; }

        public PullRequestEventContext(PullRequestPayload payload, IConnection githubConnection)
        {
            Payload = payload;
            GithubConnection = githubConnection;
        }
    }
}
