using System;
using System.Security.Cryptography.X509Certificates;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Storage;
using DevDefined.OAuth.Tests;

namespace DevDefined.OAuth.Testing
{
    public class TestConsumerStore : IConsumerStore
    {
        #region IConsumerStore Members

        public bool IsConsumer(IConsumer consumer)
        {
            return (consumer.ConsumerKey == "key" && string.IsNullOrEmpty(consumer.Realm));
        }

        public void SetConsumerSecret(IConsumer consumer, string consumerSecret)
        {
            throw new NotImplementedException();
        }

        public string GetConsumerSecret(IConsumer consumer)
        {
            return "secret";
        }

        public void SetConsumerCertificate(IConsumer consumer, X509Certificate2 certificate)
        {
            throw new NotImplementedException();
        }

        public X509Certificate2 GetConsumerCertificate(IConsumer consumer)
        {
            return TestCertificates.OAuthTestCertificate();
        }

        #endregion
    }
}