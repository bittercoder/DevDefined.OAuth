using System.Collections.Generic;
using System.Linq;
using DevDefined.OAuth.Framework;
using NUnit.Framework;
using QueryParameter = System.Collections.Generic.KeyValuePair<string, string>;

namespace DevDefined.OAuth.Tests.Core
{
    [TestFixture]
    public class UriUtilityTests
    {
        [Test]
        public void GetQueryParametersWithoutQuestionMark()
        {
            List<QueryParameter> parameters = UriUtility.GetQueryParameters("key1=value1&key2=value2");
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("value1", Enumerable.Single(parameters, p => p.Key == "key1").Value);
            Assert.AreEqual("value2", Enumerable.Single(parameters, p => p.Key == "key2").Value);
        }

        [Test]
        public void GetQueryParametersWithQuestionMark()
        {
            List<QueryParameter> parameters = UriUtility.GetQueryParameters("?key1=value1&key2=value2");
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("value1", Enumerable.Single(parameters, p => p.Key == "key1").Value);
            Assert.AreEqual("value2", Enumerable.Single(parameters, p => p.Key == "key2").Value);
        }
    }
}