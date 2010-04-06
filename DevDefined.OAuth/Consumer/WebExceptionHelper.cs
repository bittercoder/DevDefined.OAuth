using System;
using System.Net;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Consumer
{
    public static class WebExceptionHelper
    {
        /// <summary>
        /// Will attempt to wrap the exception, returning true if the exception was wrapped, or returning false if it was not (in which case
        /// the original exception should be thrown).
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="webException"></param>
        /// <param name="wrappedException">The wrapped exception (will be set to the webException, if this method returns <c>false</c></param>
        /// <returns><c>true</c>, if the authException should be throw, <c>false</c> if the wrapped web exception should be thrown</returns>
        public static bool TryWrapException(IOAuthContext requestContext, WebException webException, out Exception wrappedException)
        {
            try
            {
                string content = webException.Response.ReadToEnd();

                if (content.Contains(Parameters.OAuth_Problem))
                {
                  var report = new OAuthProblemReport(content);
                  wrappedException = new OAuthException(report.ProblemAdvice ?? report.Problem, webException) { Context = requestContext, Report = report };
                }
                else
                {
                  wrappedException = new ParsedWebException(content, webException.Message, webException.GetBaseException(), webException.Status, webException.Response);
                }

                return true;
            }
            catch
            {
              wrappedException = webException;
              return false;
            }          
        }
    }
}