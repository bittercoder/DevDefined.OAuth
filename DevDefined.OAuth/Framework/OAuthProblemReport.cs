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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace DevDefined.OAuth.Framework
{
  [Serializable]
  public class OAuthProblemReport
  {
    public OAuthProblemReport()
    {
      ParametersRejected = new List<string>();
      ParametersAbsent = new List<string>();
    }

    public OAuthProblemReport(NameValueCollection parameters)
    {
      Problem = parameters[Parameters.OAuth_Problem];

      ProblemAdvice = parameters[Parameters.OAuth_Problem_Advice];

      ParametersAbsent = parameters.AllKeys.Any(key => key == Parameters.OAuth_Parameters_Absent)
                           ? ParseFormattedParameters(parameters[Parameters.OAuth_Parameters_Absent])
                           : new List<string>();

      ParametersRejected = parameters.AllKeys.Any(key => key == Parameters.OAuth_Parameters_Rejected)
                             ? ParseFormattedParameters(parameters[Parameters.OAuth_Parameters_Rejected])
                             : new List<string>();

      if (parameters.AllKeys.Any(key => key == Parameters.OAuth_Acceptable_Timestamps))
      {
        string[] timeStamps = parameters[Parameters.OAuth_Acceptable_Timestamps].Split(new[] {'-'});
        AcceptableTimeStampsFrom = DateTimeUtility.FromEpoch(Convert.ToInt64(timeStamps[0]));
        AcceptableTimeStampsTo = DateTimeUtility.FromEpoch(Convert.ToInt64(timeStamps[1]));
      }

      if (parameters.AllKeys.Any(key => key == Parameters.OAuth_Acceptable_Versions))
      {
        string[] versions = parameters[Parameters.OAuth_Acceptable_Versions].Split(new[] {'-'});
        AcceptableVersionFrom = versions[0];
        AcceptableVersionTo = versions[1];
      }
    }

    public OAuthProblemReport(string formattedReport)
      : this(HttpUtility.ParseQueryString(formattedReport))
    {
    }

    public string AcceptableVersionTo { get; set; }
    public string AcceptableVersionFrom { get; set; }
    public List<string> ParametersRejected { get; set; }
    public List<string> ParametersAbsent { get; set; }
    public string ProblemAdvice { get; set; }
    public string Problem { get; set; }
    public DateTime? AcceptableTimeStampsTo { get; set; }
    public DateTime? AcceptableTimeStampsFrom { get; set; }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(Problem)) throw Error.CantBuildProblemReportWhenProblemEmpty();

      var response = new NameValueCollection();

      response[Parameters.OAuth_Problem] = Problem;

      if (!string.IsNullOrEmpty(ProblemAdvice))
      {
        response[Parameters.OAuth_Problem_Advice] = ProblemAdvice.Replace("\r\n", "\n").Replace("\r", "\n");
      }

      if (ParametersAbsent.Count > 0)
      {
        response[Parameters.OAuth_Parameters_Absent] = FormatParameterNames(ParametersAbsent);
      }

      if (ParametersRejected.Count > 0)
      {
        response[Parameters.OAuth_Parameters_Rejected] = FormatParameterNames(ParametersRejected);
      }

      if (AcceptableTimeStampsFrom.HasValue && AcceptableTimeStampsTo.HasValue)
      {
        response[Parameters.OAuth_Acceptable_Timestamps] = string.Format("{0}-{1}",
                                                                         AcceptableTimeStampsFrom.Value.Epoch(),
                                                                         AcceptableTimeStampsTo.Value.Epoch());
      }

      if (!(string.IsNullOrEmpty(AcceptableVersionFrom) || string.IsNullOrEmpty(AcceptableVersionTo)))
      {
        response[Parameters.OAuth_Acceptable_Versions] = string.Format("{0}-{1}", AcceptableVersionFrom,
                                                                       AcceptableVersionTo);
      }

      return UriUtility.FormatQueryString(response);
    }

    static string FormatParameterNames(IEnumerable<string> names)
    {
      var builder = new StringBuilder();

      foreach (string name in names)
      {
        if (builder.Length > 0) builder.Append("&");
        builder.Append(UriUtility.UrlEncode(name));
      }

      return builder.ToString();
    }

    static List<string> ParseFormattedParameters(string formattedList)
    {
      return formattedList.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
  }
}