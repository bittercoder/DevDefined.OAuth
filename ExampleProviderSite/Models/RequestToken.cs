using DevDefined.OAuth.Framework;

namespace ExampleProviderSite.Models
{
    /// <summary>
    /// Simple request token model, this provides information about a request token which has been issued, including
    /// who it was issued to, if the token has been used up (a request token should only be presented once), and 
    /// the associated access token (if a user has granted access to a consumer i.e. given them access).
    /// </summary>
    public class RequestToken : TokenBase
    {
        public bool AccessDenied { get; set; }
        public bool UsedUp { get; set; }
        public AccessToken AccessToken { get; set; }
    }
}