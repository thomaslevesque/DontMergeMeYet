using DontMergeMeYet.Data;
using DontMergeMeYet.Models.Github;
using DontMergeMeYet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DontMergeMeYet
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<InstallationContext>(ConfigureInstallationContext);

            services.AddScoped<IInstallationService, InstallationService>();
            services.AddSingleton<IGithubAppTokenService, GithubAppTokenService>();
            services.AddScoped<IGithubClientCache, GithubClientCache>();
            services.AddTransient<IPullRequestInfoProvider, PullRequestInfoProvider>();
            services.AddTransient<IPullRequestChecker, PullRequestChecker>();
            services.AddTransient<ICommitStatusWriter, CommitStatusWriter>();

            services.AddMvc();

            services.Configure<GithubSettings>(Configuration.GetSection("Github"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();

            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime?.ApplicationStarted.Register(() =>
            {
                var builder = new DbContextOptionsBuilder<InstallationContext>();
                ConfigureInstallationContext(builder);
                using (var context = new InstallationContext(builder.Options))
                {
                    context.Database.Migrate();
                }
            });
        }

        private void ConfigureInstallationContext(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("InstallationContext"));
        }
    }
}