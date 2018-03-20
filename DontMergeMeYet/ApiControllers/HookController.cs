using System;
using System.Linq;
using System.Threading.Tasks;
using DontMergeMeYet.Data;
using DontMergeMeYet.Models.Github.Webhooks;
using DontMergeMeYet.Services;
using Microsoft.AspNetCore.Mvc;

namespace DontMergeMeYet.ApiControllers
{
    [Produces("application/json")]
    [Route("api/hooks")]
    public class HookController : Controller
    {
        private readonly IInstallationService _installationService;
        private readonly IPullRequestInfoProvider _pullRequestInfoProvider;
        private readonly IPullRequestChecker _pullRequestChecker;
        private readonly ICommitStatusWriter _commitStatusWriter;

        public HookController(
            IInstallationService installationService,
            IPullRequestInfoProvider pullRequestInfoProvider,
            IPullRequestChecker pullRequestChecker,
            ICommitStatusWriter commitStatusWriter)
        {
            _installationService = installationService;
            _pullRequestInfoProvider = pullRequestInfoProvider;
            _pullRequestChecker = pullRequestChecker;
            _commitStatusWriter = commitStatusWriter;
        }

        [HttpPost]
        public async Task<IActionResult> Index(EventPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            switch (payload)
            {
                case InstallationEventPayload e:
                    await HandleInstallationAsync(e);
                    break;
                case PullRequestEventPayload e:
                    await HandlePullRequestAsync(e);
                    break;
            }

            return Ok();
        }

        private static readonly string[] PullRequestActions =
        {
            "labeled",
            "unlabeled",
            "opened",
            "edited",
            "reopened",
            "synchronize"
        };

        

        private async Task HandlePullRequestAsync(PullRequestEventPayload payload)
        {
            if (!PullRequestActions.Contains(payload.Action))
                return;

            var installation =
                await _installationService.GetInstallationForRepositoryAsync(payload.Repository.Id);
            if (installation == null)
                return;

            var prInfo = await _pullRequestInfoProvider.GetPullRequestInfoAsync(installation.InstallationId, payload);
            var status = _pullRequestChecker.GetStatus(prInfo);
            await _commitStatusWriter.WriteCommitStatusAsync(
                installation.InstallationId,
                payload.Repository.FullName,
                prInfo.Head,
                status);
        }

        private async Task HandleInstallationAsync(InstallationEventPayload payload)
        {
            if (payload.Action == "created")
            {
                var repoInstalls =
                    payload.Repositories
                        .Select(repo => new RepositoryInstallation
                        {
                            RepositoryId = repo.Id,
                            RepositoryFullName = repo.FullName,
                            InstallationId = payload.Installation.Id,
                            InstallationDate = DateTime.UtcNow
                        });

                await _installationService.InstallAsync(repoInstalls);
            }
            else if (payload.Action == "deleted")
            {
                await _installationService.UninstallAsync(payload.Installation.Id);
            }
        }
    }
}