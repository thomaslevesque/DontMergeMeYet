namespace DontMergeMeYet.Services.Abstractions
{
    public interface IGithubPayloadValidator
    {
        bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature);
    }
}
