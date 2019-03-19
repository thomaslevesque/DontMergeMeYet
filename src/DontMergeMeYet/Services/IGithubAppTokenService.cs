using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    interface IGithubAppTokenService
    {
        Task<string> GetTokenForApplicationAsync();
        Task<string> GetTokenForInstallationAsync(long installationId);
    }
}
