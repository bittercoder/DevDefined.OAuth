using System;
using DevDefined.OAuth.Core;

namespace ExampleProviderSite.Models
{
    /// <summary>
    /// Simple access token model, this would hold information required to enforce policies such as expiration, and association
    /// with a user accout or other information regarding the information the consumer has been granted access to.
    /// </summary>
    public class AccessToken : TokenBase
    {
        public string UserName { get; set; }
        public DateTime ExpireyDate { get; set; }
    }
}