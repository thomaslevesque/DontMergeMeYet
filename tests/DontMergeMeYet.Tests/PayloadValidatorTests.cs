using System;
using System.Linq;
using DontMergeMeYet.Services;
using FakeItEasy;
using Shouldly;
using Xunit;

namespace DontMergeMeYet.Tests
{
    public class PayloadValidatorTests : TestFixtureBase
    {
        private readonly IGithubSettingsProvider _settingsProvider;

        private readonly GithubPayloadValidator _validator;

        public PayloadValidatorTests()
        {
            InitFake(out _settingsProvider);
            A.CallTo(() => _settingsProvider.Settings)
                .Returns(new GithubSettings("", "foobarbaz", "", ""));

            _validator = new GithubPayloadValidator(_settingsProvider);
        }

        [Theory]
        [InlineData("c0d79071d7a12e78fbc84f9acd9414cc", "sha1=876b083cdcb6a47b77ebcb0263b4ffb3fc6386b4")]
        [InlineData("9663078bf4cc747bf31668739e8e635f0539e2d3b08f9689a80e4e8cc16cc4d0", "sha1=8b8b2a91700dca346fcd03659f0cf6ba226ea2da")]
        [InlineData("bd1efd0225f0528a940ea48f274ec5d5b042ade5608c5d1b7588d5d70c588ea3f492937185cbdf154bbe75671f8353db22ed", "sha1=e8d8a07fc49df91e6b48364b24eb1187609ec875")]
        public void IsPayloadSignatureValid_returns_true_for_valid_signature(string hexBytes, string receivedSignature)
        {
            var bytes = FromHexString(hexBytes);
            _validator.IsPayloadSignatureValid(bytes, receivedSignature).ShouldBeTrue();
        }

        [Theory]
        [InlineData("c0d79071d7a12e78fbc84f9acd9414cc", "sha1=076b083cdcb6a47b77ebcb0263b4ffb3fc6386b4")]
        [InlineData("9663078bf4cc747bf31668739e8e635f0539e2d3b08f9689a80e4e8cc16cc4d0", "sha1=0b8b2a91700dca346fcd03659f0cf6ba226ea2da")]
        [InlineData("bd1efd0225f0528a940ea48f274ec5d5b042ade5608c5d1b7588d5d70c588ea3f492937185cbdf154bbe75671f8353db22ed", "sha1=08d8a07fc49df91e6b48364b24eb1187609ec875")]
        public void IsPayloadSignatureValid_returns_false_for_valid_signature(string hexBytes, string receivedSignature)
        {
            var bytes = FromHexString(hexBytes);
            _validator.IsPayloadSignatureValid(bytes, receivedSignature).ShouldBeFalse();
        }


        private static byte[] FromHexString(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Invalid length", nameof(hex));

            return Enumerable.Range(0, hex.Length / 2)
                .Select(i => HexToByte(hex, i * 2))
                .ToArray();
        }

        private static byte HexToByte(string hex, int position)
        {
            return (byte)(GetHexDigitValue(hex[position]) * 16 + GetHexDigitValue(hex[position + 1]));
        }

        private static byte GetHexDigitValue(char c)
        {
            if (c >= '0' && c <= '9')
                return (byte)(c - '0');

            if (c >= 'a' && c <= 'f')
                return (byte)(10 + c - 'a');

            if (c >= 'A' && c <= 'F')
                return (byte)(10 + c - 'A');

            throw new ArgumentException("Invalid hex digit", nameof(c));
        }
    }
}
