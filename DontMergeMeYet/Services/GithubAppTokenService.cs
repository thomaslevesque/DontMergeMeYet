using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DontMergeMeYet.Models.Github.Webhooks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace DontMergeMeYet.Services
{
    class GithubAppTokenService : IGithubAppTokenService
    {
        private readonly GithubSettings _settings = GetGithubSettings();

        public Task<string> GetTokenForApplicationAsync()
        {
            var key = new RsaSecurityKey(_settings.RsaParameters);
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(claims: new[]
                {
                    new Claim("iat", now.ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                    new Claim("exp", now.AddMinutes(10).ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                    new Claim("iss", _settings.AppId)
                },
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(jwt);
        }

        public async Task<string> GetTokenForInstallationAsync(int installationId)
        {
            var appToken = await GetTokenForApplicationAsync();
            using (var client = new HttpClient())
            {
                string url = $"https://api.github.com/installations/{installationId}/access_tokens";
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue("Bearer", appToken),
                        UserAgent =
                        {
                            ProductInfoHeaderValue.Parse("DontMergeMeYet"),
                        },
                        Accept =
                        {
                            MediaTypeWithQualityHeaderValue.Parse("application/vnd.github.machine-man-preview+json")
                        }
                    }
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(json);
                    return obj["token"]?.Value<string>();
                }
            }
        }


        private static GithubSettings GetGithubSettings()
        {
            return new GithubSettings
            {
                AppId = ConfigurationManager.AppSettings["GithubAppId"],
                WebhookSecret = ConfigurationManager.AppSettings["GithubWebhookSecret"],
                PrivateKey = ConfigurationManager.AppSettings["GithubPrivateKey"]
            };
        }
    }
}