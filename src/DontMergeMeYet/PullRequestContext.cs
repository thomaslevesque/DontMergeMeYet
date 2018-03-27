using Microsoft.Azure.WebJobs.Host;
using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestContext
    {
        public PullRequestPayload Payload { get; }
        public IConnection GithubConnection { get; }
        public TraceWriter Log { get; }

        public PullRequestContext(PullRequestPayload payload, IConnection githubConnection, TraceWriter log)
        {
            Payload = payload;
            GithubConnection = githubConnection;
            Log = log;
        }
    }
}
