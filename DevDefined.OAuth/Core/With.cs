using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DevDefined.OAuth.Core
{
    public static class With
    {
        public static IDisposable NoCertificateValidation()
        {
            var oldCallback = System.Net.ServicePointManager.ServerCertificateValidationCallback;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateAlwaysValidCallback;
            return new DisposableAction(() => System.Net.ServicePointManager.ServerCertificateValidationCallback = oldCallback);
        }

        private static bool CertificateAlwaysValidCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
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

        public void Dispose()
        {
            _action();
        }
    }
}
