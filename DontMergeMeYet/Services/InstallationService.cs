using System.Collections.Generic;
using System.Threading.Tasks;
using DontMergeMeYet.Data;
using Microsoft.EntityFrameworkCore;

namespace DontMergeMeYet.Services
{
    class InstallationService : IInstallationService
    {
        private readonly InstallationContext _context;

        public InstallationService(InstallationContext context)
        {
            _context = context;
        }

        public async Task InstallAsync(IEnumerable<RepositoryInstallation> repositoryInstallations)
        {
            foreach (var repositoryInstallation in repositoryInstallations)
            {
                await InstallCoreAsync(repositoryInstallation);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UninstallAsync(int installationId)
        {
            await _context.Database.ExecuteSqlCommandAsync(
                "DELETE FROM RepositoryInstallation WHERE InstallationId = @installationId",
                installationId);
        }

        public async Task UninstallAsync(int installationId, IEnumerable<int> repositoryIds)
        {
            await _context.Database.ExecuteSqlCommandAsync(
                "DELETE FROM RepositoryInstallation WHERE InstallationId = @installationId AND RepositoryId IN (@repositoryIds)",
                installationId,
                repositoryIds);
        }

        public async Task<RepositoryInstallation> GetInstallationForRepositoryAsync(int repositoryId)
        {
            var existing =
                await _context.RepositoryInstallations.FirstOrDefaultAsync(i => i.RepositoryId == repositoryId);
            return existing;
        }

        private async Task InstallCoreAsync(RepositoryInstallation repositoryInstallation)
        {
            var existing =
                await _context.RepositoryInstallations.FirstOrDefaultAsync(
                    i => i.RepositoryId == repositoryInstallation.RepositoryId
                         && i.InstallationId == repositoryInstallation.InstallationId);

            if (existing != null)
            {
                existing.RepositoryFullName = repositoryInstallation.RepositoryFullName;
                existing.InstallationId = repositoryInstallation.InstallationId;
                existing.InstallationDate = repositoryInstallation.InstallationDate;
                _context.RepositoryInstallations.Update(existing);
            }
            else
            {
                await _context.RepositoryInstallations.AddAsync(repositoryInstallation);
            }
        }
    }
}