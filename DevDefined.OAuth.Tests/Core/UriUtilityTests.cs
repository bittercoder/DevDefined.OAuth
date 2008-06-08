using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DevDefined.OAuth.Core;

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
            Assert.AreEqual("value1", parameters.Single(p => p.Name == "key1").Value);
            Assert.AreEqual("value2", parameters.Single(p => p.Name == "key2").Value);
        }

        [Test]
        public void GetQueryParametersWithQuestionMark()
        {
            List<QueryParameter> parameters = UriUtility.GetQueryParameters("?key1=value1&key2=value2");
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("value1", parameters.Single(p => p.Name == "key1").Value);
            Assert.AreEqual("value2", parameters.Single(p => p.Name == "key2").Value);
        }
    }
}
