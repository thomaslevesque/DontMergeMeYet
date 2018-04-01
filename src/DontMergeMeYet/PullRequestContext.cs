using Microsoft.Azure.WebJobs.Host;
using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestContext
    {
        public PullRequestContext(PullRequestPayload payload, IConnection githubConnection, TraceWriter log)
        {
            Payload = payload;
            GithubConnection = githubConnection;
            Log = log;
        }

        public PullRequestPayload Payload { get; }
        public IConnection GithubConnection { get; }
        public TraceWriter Log { get; }
        public PullRequestInfo PullRequestInfo { get; set; }
        public RepositorySettings RepositorySettings { get; set; }
    }
}
