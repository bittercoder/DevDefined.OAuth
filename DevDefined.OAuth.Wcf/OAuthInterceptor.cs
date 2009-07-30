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
using System.IdentityModel.Policy;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web;
using System.Xml.Linq;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider;
using DevDefined.OAuth.Storage.Basic;
using Microsoft.ServiceModel.Web;

namespace DevDefined.OAuth.Wcf
{
  public class OAuthInterceptor : RequestInterceptor
  {
    readonly IOAuthProvider _provider;
    readonly ITokenRepository<AccessToken> _repository;

    public OAuthInterceptor(IOAuthProvider provider, ITokenRepository<AccessToken> repository)
      : base(false)
    {
      if (provider == null) throw new ArgumentNullException("provider");
      if (repository == null) throw new ArgumentNullException("repository");
      _provider = provider;
      _repository = repository;
    }

    public override void ProcessRequest(ref RequestContext requestContext)
    {
      if (requestContext == null || requestContext.RequestMessage == null)
      {
        return;
      }

      Message request = requestContext.RequestMessage;

      var requestProperty = (HttpRequestMessageProperty) request.Properties[HttpRequestMessageProperty.Name];

      IOAuthContext context = new OAuthContextBuilder().FromUri(requestProperty.Method, request.Headers.To);

      try
      {
        _provider.AccessProtectedResourceRequest(context);

        AccessToken accessToken = _repository.GetToken(context.Token);

        TokenPrincipal principal = CreatePrincipalFromToken(accessToken);

        InitializeSecurityContext(request, principal);
      }
      catch (OAuthException authEx)
      {
        XElement response = GetHtmlFormattedErrorReport(authEx);
        Message reply = Message.CreateMessage(MessageVersion.None, null, response);
        var responseProperty = new HttpResponseMessageProperty {StatusCode = HttpStatusCode.Forbidden, StatusDescription = authEx.Report.ToString()};
        responseProperty.Headers[HttpResponseHeader.ContentType] = "text/html";
        reply.Properties[HttpResponseMessageProperty.Name] = responseProperty;
        requestContext.Reply(reply);

        requestContext = null;
      }
    }

    static TokenPrincipal CreatePrincipalFromToken(AccessToken accessToken)
    {
      return new TokenPrincipal(
        new GenericIdentity(accessToken.UserName, "OAuth"),
        accessToken.Roles,
        accessToken);
    }

    static XElement GetHtmlFormattedErrorReport(OAuthException authEx)
    {
      // TODO: Review OAuth error reporting extension, I don't think it allows for html formatting of the error report.

      string reportAsHtmlDocument = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<html xmlns=\"http://www.w3.org/1999/xhtml\" version=\"-//W3C//DTD XHTML 2.0//EN\" xml:lang=\"en\" " +
                                    "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                                    "xsi:schemaLocation=\"http://www.w3.org/1999/xhtml http://www.w3.org/MarkUp/SCHEMA/xhtml2.xsd\">" +
                                    "<HEAD><TITLE>Request Error</TITLE></HEAD><BODY><DIV id=\"content\"><P class=\"heading1\"><B>" +
                                    HttpUtility.HtmlEncode(authEx.Report.ToString()) +
                                    "</B></P></DIV></BODY></html>";

      return XElement.Load(new StringReader(reportAsHtmlDocument));
    }

    static void InitializeSecurityContext(Message request, IPrincipal principal)
    {
      var policies = new List<IAuthorizationPolicy> {new PrincipalAuthorizationPolicy(principal)};
      var securityContext = new ServiceSecurityContext(policies.AsReadOnly());

      if (request.Properties.Security != null)
      {
        request.Properties.Security.ServiceSecurityContext = securityContext;
      }
      else
      {
        request.Properties.Security = new SecurityMessageProperty {ServiceSecurityContext = securityContext};
      }
    }
  }
}