using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace DevDefined.OAuth.Core
{
    public class OAuthProblemReport
    {
        public OAuthProblemReport()
        {
            ParametersRejected = new List<string>();
            ParametersAbsent = new List<string>();
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

        private string FormatParameterNames(List<string> names)
        {
            var builder = new StringBuilder();

            foreach (string name in names)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.Append(UriUtility.UrlEncode(name));
            }

            return builder.ToString();
        }
    }
}