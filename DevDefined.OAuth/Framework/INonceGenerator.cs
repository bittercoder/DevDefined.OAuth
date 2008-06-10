using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth
{
    /// <summary>
    /// Generates a nonce, which should be unique for the selected consumer (i.e. never generated
    /// by subsequent calls to <see cref="GenerateNonce" />)
    /// </summary>
    public interface INonceGenerator
    {
        string GenerateNonce(OAuthContext context);
    }
}