using System;
using System.Net;

namespace DevDefined.OAuth.Framework
{
    /// <summary>
    /// Extends WebException.  Calls to the underlying Response.GetResponseStream() will fail since the response body has already been read, 
    /// so the body is included in the property <see cref="ResponseContent" /> instead.
    /// </summary>
    public class ParsedWebException : WebException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.WebException"/> class with the specified error message, nested exception, status, and response.
        /// </summary>
        /// <param name="content">The resposne body content</param>
        /// <param name="message">The text of the error message.</param>
        /// <param name="innerException">A nested exception.</param>
        /// <param name="status">One of the <see cref="T:System.Net.WebExceptionStatus"/> values.</param>
        /// <param name="response">A <see cref="T:System.Net.WebResponse"/> instance that contains the response from the remote host.</param>
        public ParsedWebException(string content, string message, Exception innerException, WebExceptionStatus status, WebResponse response) 
          : base(message, innerException, status, response)
        {
            ResponseContent = content;
        }

        /// <summary>
        /// The response body content
        /// </summary>
        public string ResponseContent { get; private set; }
    }
}
