using System;
using System.IO;
using System.Net;
using System.Web;

namespace DevDefined.OAuth.Framework
{
	public interface IOAuthContextBuilder
	{
		IOAuthContext FromUrl(string httpMethod, string url);
		IOAuthContext FromUri(string httpMethod, Uri uri);
		IOAuthContext FromHttpRequest(HttpRequest request);
		IOAuthContext FromHttpRequest(HttpRequestBase request);
		IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody);
		IOAuthContext FromWebRequest(HttpWebRequest request, string body);
	}
}