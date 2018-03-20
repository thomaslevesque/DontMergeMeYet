using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DontMergeMeYet.Services
{
    interface IGithubClientCache : IDisposable
    {
        Task<HttpClient> GetClientForInstallationAsync(int installationId);
    }
}
