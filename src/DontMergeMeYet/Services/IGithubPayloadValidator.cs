namespace DontMergeMeYet.Services
{
    public interface IGithubPayloadValidator
    {
        bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature);
    }
}
