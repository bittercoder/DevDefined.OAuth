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
    /// <param name="webEx"></param>
    /// <param name="authException"></param>
    /// <returns><c>true</c>, if the authException should be throw, <c>false</c> if the original web exception should be thrown</returns>
    public static bool TryWrapException(IOAuthContext requestContext, WebException webEx, out OAuthException authException)
    {
      try
      {
        string content = webEx.Response.ReadToEnd();
        if (content.Contains(Parameters.OAuth_Problem))
        {
          var report = new OAuthProblemReport(content);
          authException = new OAuthException(report.ProblemAdvice ?? report.Problem, webEx) {Context = requestContext, Report = report};
          return true;
        }
      }
      catch
      {
      }
      authException = new OAuthException();
      return false;
    }
  }
}