using System.Threading.Tasks;
using DontMergeMeYet.Services;
using FakeItEasy;
using Octokit;
using Xunit;

namespace DontMergeMeYet.Tests
{
    public class PullRequestHandlerTests : TestFixtureBase
    {
        private readonly IPullRequestInfoProvider _prInfoProvider;
        private readonly IPullRequestPolicy _pullRequestPolicy;
        private readonly ICommitStatusWriter _statusWriter;

        private readonly PullRequestHandler _handler;

        public PullRequestHandlerTests()
        {
            InitFake(out _prInfoProvider);
            InitFake(out _pullRequestPolicy);
            InitFake(out _statusWriter);

            _handler = new PullRequestHandler(_prInfoProvider, _pullRequestPolicy, _statusWriter);
        }

        [Fact]
        public async Task HandleWebhookEventAsync_should_write_status_returned_by_policy()
        {
            // Arrange
            A.CallTo(() => _pullRequestPolicy.GetStatus(A<PullRequestContext>._, A<PullRequestInfo>._))
                .Returns((CommitState.Pending, "blah"));

            // Act
            var context = A.Dummy<PullRequestContext>();
            await _handler.HandleWebhookEventAsync(context);

            // Assert
            A.CallTo(() => _statusWriter.WriteCommitStatusAsync(context, CommitState.Pending, "blah"))
                .MustHaveHappenedOnceExactly();
        }
    }
}
