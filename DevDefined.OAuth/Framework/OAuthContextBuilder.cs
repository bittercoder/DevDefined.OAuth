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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.ServiceModel.Channels;

namespace DevDefined.OAuth.Framework
{
  public interface IOAuthContextBuilder
  {
    IOAuthContext FromUri(string httpMethod, Uri uri);
    IOAuthContext FromHttpRequest(HttpRequest request);
    IOAuthContext FromHttpRequest(HttpRequestBase request);
    IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody);
    IOAuthContext FromWebRequest(HttpWebRequest request, string body);
    IOAuthContext FromMessage(Message request);
  }

  public class OAuthContextBuilder : IOAuthContextBuilder
  {
    public IOAuthContext FromUri(string httpMethod, Uri uri)
    {
      uri = CleanUri(uri);

      if (httpMethod == null) throw new ArgumentNullException("httpMethod");
      if (uri == null) throw new ArgumentNullException("uri");

      return new OAuthContext
        {
          RawUri = uri,
          RequestMethod = httpMethod
        };
    }

    public IOAuthContext FromHttpRequest(HttpRequest request)
    {
      return FromHttpRequest(new HttpRequestWrapper(request));
    }

    public IOAuthContext FromMessage(Message request)
    {
        var requestProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        var context = new OAuthContext
        {
            RawUri = CleanUri(request.Headers.To),
            Headers = new NameValueCollection(requestProperty.Headers),
            RequestMethod = requestProperty.Method,
            QueryParameters = HttpUtility.ParseQueryString(requestProperty.QueryString),
        };

        string authHeader = requestProperty.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authHeader))
        {
            context.AuthorizationHeaderParameters = UriUtility.GetHeaderParameters(authHeader).ToNameValueCollection();
            context.UseAuthorizationHeader = true;
        }

        return context;
    }

    public IOAuthContext FromHttpRequest(HttpRequestBase request)
    {
      var context = new OAuthContext
        {
          RawUri = CleanUri(request.Url),
          Cookies = CollectCookies(request),
          Headers = new NameValueCollection(request.Headers),
          RequestMethod = request.HttpMethod,
          FormEncodedParameters = new NameValueCollection(request.Form),
          QueryParameters = new NameValueCollection(request.QueryString),
        };
      if (request.Headers.AllKeys.Contains("Authorization"))
      {
        context.AuthorizationHeaderParameters = UriUtility.GetHeaderParameters(request.Headers["Authorization"]).ToNameValueCollection();
        context.UseAuthorizationHeader = true;
      }

      return context;
    }

    public IOAuthContext FromWebRequest(HttpWebRequest request, Stream rawBody)
    {
      using (var reader = new StreamReader(rawBody))
      {
        return FromWebRequest(request, reader.ReadToEnd());
      }
    }

    public IOAuthContext FromWebRequest(HttpWebRequest request, string body)
    {
      var context = new OAuthContext
        {
          RawUri = CleanUri(request.RequestUri),
          Cookies = CollectCookies(request),
          Headers = request.Headers,
          RequestMethod = request.Method
        };

      if (request.Headers[HttpRequestHeader.ContentType] == "application/x-www-form-urlencoded")
      {
        context.FormEncodedParameters = HttpUtility.ParseQueryString(body);
      }

      return context;
    }

    static Uri CleanUri(Uri uri)
    {
      // this is a fix for OpenSocial platforms sometimes appending an empty query string parameter
      // to their url.

      string originalUrl = uri.OriginalString;
      return originalUrl.EndsWith("&") ? new Uri(originalUrl.Substring(0, originalUrl.Length - 1)) : uri;
    }

    static NameValueCollection CollectCookies(WebRequest request)
    {
      return CollectCookiesFromHeaderString(request.Headers[HttpRequestHeader.Cookie]);
    }

    static NameValueCollection CollectCookies(HttpRequestBase request)
    {
      return CollectCookiesFromHeaderString(request.Headers["Set-Cookie"]);
    }

    static NameValueCollection CollectCookiesFromHeaderString(string cookieHeader)
    {
      var cookieCollection = new NameValueCollection();

      if (!string.IsNullOrEmpty(cookieHeader))
      {
        string[] cookies = cookieHeader.Split(';');
        foreach (string cookie in cookies)
        {
          //Remove the trailing and Leading white spaces
          string strCookie = cookie.Trim();

          var reg = new Regex(@"^(\S*)=(\S*)$");
          if (reg.IsMatch(strCookie))
          {
            Match match = reg.Match(strCookie);
            if (match.Groups.Count > 2)
            {
              cookieCollection.Add(match.Groups[1].Value,
                                   HttpUtility.UrlDecode(match.Groups[2].Value).Replace(' ', '+'));
              //HACK: find out why + is coming in as " "
            }
          }
        }
      }

      return cookieCollection;
    }
  }
}