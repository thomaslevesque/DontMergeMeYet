using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DontMergeMeYet.Extensions;
using DontMergeMeYet.Services.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace DontMergeMeYet.Services
{
    class GithubAppTokenService : IGithubAppTokenService
    {
        private readonly GithubSettings _settings;
        private readonly SigningCredentials _signingCredentials;

        public GithubAppTokenService(IOptions<GithubSettings> options)
        {
            _settings = options.Value;
            var rsaParameters = CryptoHelper.GetRsaParameters(_settings.PrivateKey);
            var key = new RsaSecurityKey(rsaParameters);
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        }

        public Task<string> GetTokenForApplicationAsync()
        {
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(claims: new[]
                {
                    new Claim("iat", now.ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                    new Claim("exp", now.AddMinutes(10).ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                    new Claim("iss", _settings.AppId)
                },
                signingCredentials: _signingCredentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(jwt);
        }

        public async Task<string> GetTokenForInstallationAsync(long installationId)
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
    }
}