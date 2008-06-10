using System;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Core
{
    [TestFixture]
    public class OAuthProblemReportTests
    {
        [Test]
        public void FormatMissingParameterReport()
        {
            var report = new OAuthProblemReport
                             {
                                 Problem = OAuthProblems.ParameterAbset,
                                 ParametersAbsent = {Parameters.OAuth_Nonce}
                             };


            Assert.AreEqual("oauth_problem=parameter_absent&oauth_parameters_absent=oauth_nonce", report.ToString());
        }

        [Test]
        public void FormatRejectedParameterReport()
        {
            var report = new OAuthProblemReport
                             {
                                 Problem = OAuthProblems.ParameterRejected,
                                 ParametersRejected = {Parameters.OAuth_Timestamp}
                             };

            Assert.AreEqual("oauth_problem=parameter_rejected&oauth_parameters_rejected=oauth_timestamp",
                            report.ToString());
        }

        [Test]
        public void FormatReportWithAdvice()
        {
            var report = new OAuthProblemReport
                             {
                                 Problem = OAuthProblems.ConsumerKeyRefused,
                                 ProblemAdvice = "The supplied consumer key has been black-listed due to complaints."
                             };

            Assert.AreEqual(
                "oauth_problem=consumer_key_refused&oauth_problem_advice=The%20supplied%20consumer%20key%20has%20been%20black-listed%20due%20to%20complaints.",
                report.ToString());
        }

        [Test]
        public void FormatTimestampRangeReport()
        {
            var report = new OAuthProblemReport
                             {
                                 Problem = OAuthProblems.TimestampRefused,
                                 AcceptableTimeStampsFrom = new DateTime(2008, 1, 1),
                                 AcceptableTimeStampsTo = new DateTime(2009, 1, 1)
                             };


            Assert.AreEqual("oauth_problem=timestamp_refused&oauth_acceptable_timestamps=1199098800-1230721200",
                            report.ToString());
        }

        [Test]
        public void FormatVersionRangeReport()
        {
            var report = new OAuthProblemReport
                             {
                                 Problem = OAuthProblems.VersionRejected,
                                 AcceptableVersionFrom = "1.0",
                                 AcceptableVersionTo = "2.0"
                             };

            Assert.AreEqual("oauth_problem=version_rejected&oauth_acceptable_versions=1.0-2.0", report.ToString());
        }
    }
}