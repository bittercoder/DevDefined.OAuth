using System;
using System.Runtime.Serialization;

namespace DevDefined.OAuth.Core
{   
    public class OAuthException : Exception
    {
        public OAuthException()
        {
        }

        public OAuthException(OAuthContext context, string problem, string advice) : base(advice)
        {
            Context = context;
            Report = new OAuthProblemReport {Problem = problem, ProblemAdvice = advice};
        }

        public OAuthException(OAuthContext context, string problem, string advice, Exception innerException)
            : base(advice, innerException)
        {
            Context = context;
            Report = new OAuthProblemReport {Problem = problem, ProblemAdvice = advice};
        }

        public OAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OAuthProblemReport Report { get; set; }
        public OAuthContext Context { get; set; }        
    }
}