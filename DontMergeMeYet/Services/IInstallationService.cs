using System.Collections.Generic;
using System.Threading.Tasks;
using DontMergeMeYet.Data;

namespace DontMergeMeYet.Services
{
    public interface IInstallationService
    {
        Task InstallAsync(IEnumerable<RepositoryInstallation> repositoryInstallations);
        Task UninstallAsync(int installationId);
        Task UninstallAsync(int installationId, IEnumerable<int> repositoryIds);
        Task<RepositoryInstallation> GetInstallationForRepositoryAsync(int repositoryId);
    }
}
