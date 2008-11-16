#region License

// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using DevDefined.OAuth.Framework;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Framework
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