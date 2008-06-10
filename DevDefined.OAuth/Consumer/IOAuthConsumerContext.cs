using System.Security.Cryptography;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Consumer
{
    /// <summary>
    /// A consumer context is used to identify a consumer, and to sign a context on behalf 
    /// of a consumer using an optional supplied token.
    /// </summary>
    public interface IOAuthConsumerContext
    {
        string Realm { get; set; }
        string ConsumerKey { get; set; }
        string ConsumerSecret { get; set; }
        string SignatureMethod { get; set; }
        AsymmetricAlgorithm Key { get; set; }
        bool UseHeaderForOAuthParameters { get; set; }
        void SignContext(OAuthContext context);
        void SignContextWithToken(OAuthContext context, IToken token);
    }
}