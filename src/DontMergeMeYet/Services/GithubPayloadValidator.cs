using System;
using System.Security.Cryptography;
using System.Text;
using DontMergeMeYet.Extensions;

namespace DontMergeMeYet.Services
{
    public class GithubPayloadValidator : IGithubPayloadValidator
    {
        private readonly IGithubSettingsProvider _settingsProvider;

        public GithubPayloadValidator(IGithubSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature)
        {
            if (string.IsNullOrEmpty(receivedSignature))
                return false;

            var key = Encoding.ASCII.GetBytes(_settingsProvider.Settings.WebhookSecret);
            var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(bytes);
            var actualSignature = "sha1=" + hash.ToHexString();
            return SecureEquals(actualSignature, receivedSignature);
        }

        // Constant-time comparison
        private bool SecureEquals(string a, string b)
        {
            int len = Math.Min(a.Length, b.Length);
            bool equals = a.Length == b.Length;
            for (int i = 0; i < len; i++)
            {
                equals &= (a[i] == b[i]);
            }

            return equals;
        }
    }
}