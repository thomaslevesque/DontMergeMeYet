using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestContext
    {
        public PullRequestPayload Payload { get; }
        public IConnection GithubConnection { get; }

        public PullRequestContext(PullRequestPayload payload, IConnection githubConnection)
        {
            Payload = payload;
            GithubConnection = githubConnection;
        }
    }
}
