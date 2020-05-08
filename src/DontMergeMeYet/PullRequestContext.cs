using Microsoft.Extensions.Logging;
using Octokit;

namespace DontMergeMeYet
{
    public class PullRequestContext
    {
        public PullRequestContext(string repositoryName, PullRequestEventPayload payload, IConnection githubConnection, ILogger logger)
        {
            RepositoryName = repositoryName;
            Payload = payload;
            GithubConnection = githubConnection;
            Logger = logger;
        }

        public string RepositoryName { get; }
        public PullRequestEventPayload Payload { get; }
        public IConnection GithubConnection { get; }
        public ILogger Logger { get; }
        public PullRequestInfo PullRequestInfo { get; set; }
        public RepositorySettings RepositorySettings { get; set; }
    }
}
