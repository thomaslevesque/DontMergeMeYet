using System.Threading.Tasks;

namespace DontMergeMeYet.Services.Abstractions
{
    interface IGithubAppTokenService
    {
        Task<string> GetTokenForApplicationAsync();
        Task<string> GetTokenForInstallationAsync(long installationId);
    }
}
