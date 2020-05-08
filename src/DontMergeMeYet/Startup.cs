using DontMergeMeYet.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(DontMergeMeYet.Startup))]

namespace DontMergeMeYet
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var rootDir = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot");
            if (string.IsNullOrEmpty(rootDir))
                rootDir = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site", "wwwroot");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(rootDir)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = builder.Services;
            services.Configure<GithubSettings>(configuration.GetSection("Github"));
            services.AddMemoryCache();
            services.AddScoped<IGithubAppTokenService, GithubAppTokenService>();
            services.AddScoped<IPullRequestInfoProvider, PullRequestInfoProvider>();
            services.AddScoped<IRepositorySettingsProvider, RepositorySettingsProvider>();
            services.AddScoped<IGithubConnectionCache, GithubConnectionCache>();
            services.AddScoped<ICommitStatusWriter, CommitStatusWriter>();
            services.AddScoped<IGithubPayloadValidator, GithubPayloadValidator>();
            services.AddTransient<IPullRequestPolicy, WorkInProgressPullRequestPolicy>();
            services.AddTransient<IPullRequestHandler, PullRequestHandler>();
        }
    }
}
