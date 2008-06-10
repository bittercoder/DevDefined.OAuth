using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DevDefined.OAuth.Framework
{
    public static class With
    {
        public static IDisposable NoCertificateValidation()
        {
            RemoteCertificateValidationCallback oldCallback = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = CertificateAlwaysValidCallback;
            return new DisposableAction(() => ServicePointManager.ServerCertificateValidationCallback = oldCallback);
        }

        private static bool CertificateAlwaysValidCallback(object sender, X509Certificate certificate, X509Chain chain,
                                                           SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        public DisposableAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _action();
        }

        #endregion
    }
}