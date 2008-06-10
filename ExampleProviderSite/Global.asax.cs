using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Provider.Inspectors;
using DevDefined.OAuth.Storage;
using DevDefined.OAuth.Testing;
using ExampleProviderSite.Implementation;
using ExampleProviderSite.Repositories;

namespace ExampleProviderSite
{
    public interface IOAuthServices
    {
        IOAuthProvider Provider { get; }
        TokenRepository TokenRepository { get; }
    }

    public static class OAuthServicesLocator
    {
        public static IOAuthServices Services
        {
            get { return (HttpContext.Current.ApplicationInstance as IOAuthServices); }
        }
    }

    public class Global : System.Web.HttpApplication, IOAuthServices
    {
        private static IOAuthProvider _provider;
        private static TokenRepository _repository;

        protected void Application_Start(object sender, EventArgs e)
        {
            var consumerStore = new TestConsumerStore();
            var nonceStore = new TestNonceStore();
            _repository = new TokenRepository();
            var tokenStore = new SimpleTokenStore(_repository);

            _provider = new OAuthProvider(tokenStore,
                new SignatureValidationInspector(consumerStore),
                new NonceStoreInspector(nonceStore),
                new TimestampRangeInspector(new TimeSpan(1, 0, 0)),
                new ConsumerValidationInspector(consumerStore)); 
        }

        public IOAuthProvider Provider
        {
            get { return _provider; }
        }

        public TokenRepository TokenRepository
        {
            get { return _repository; }
        }
    }
}