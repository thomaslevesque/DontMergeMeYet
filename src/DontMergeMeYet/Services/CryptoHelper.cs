using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace DontMergeMeYet.Services
{
    static class CryptoHelper
    {
        public static RSAParameters GetRsaParameters(string privateKeyBase64)
        {
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
            var privateKeyString = Encoding.UTF8.GetString(privateKeyBytes);
            PemReader pemReader = new PemReader(new StringReader(privateKeyString));
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair) pemReader.ReadObject();
            return DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters) keyPair.Private);
        }
    }
}
