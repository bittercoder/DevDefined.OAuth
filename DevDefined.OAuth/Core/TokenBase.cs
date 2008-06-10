namespace DevDefined.OAuth.Framework
{
    public class TokenBase : IToken
    {
        #region IToken Members

        public string TokenSecret { get; set; }

        public string Token { get; set; }

        public string ConsumerKey { get; set; }

        public string Realm { get; set; }

        #endregion

        public override string ToString()
        {
            return UriUtility.FormatTokenForResponse(this);
        }
    }
}