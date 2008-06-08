using System.Collections.Generic;
using DevDefined.OAuth.Core;
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Testing
{
    /// <summary>
    /// A simple nonce store that just tracks all nonces by consumer key in memory.
    /// </summary>
    public class TestNonceStore : INonceStore
    {
        private readonly Dictionary<string, List<string>> _nonces = new Dictionary<string, List<string>>();

        #region INonceStore Members

        public bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce)
        {
            List<string> list = GetNonceListForConsumer(consumer.ConsumerKey);
            lock (list)
            {
                if (list.Contains(nonce)) return false;
                list.Add(nonce);
                return true;
            }
        }

        #endregion

        private List<string> GetNonceListForConsumer(string consumerKey)
        {
            var list = new List<string>();

            if (!_nonces.TryGetValue(consumerKey, out list))
            {
                lock (_nonces)
                {
                    if (!_nonces.TryGetValue(consumerKey, out list))
                    {
                        list = new List<string>();
                        _nonces[consumerKey] = list;
                    }
                }
            }

            return list;
        }
    }
}