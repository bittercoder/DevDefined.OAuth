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
using System.Net;
using System.Web;
using System.Xml.Linq;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  public class ConsumerRequest : IConsumerRequest
  {
    readonly IOAuthConsumerContext _consumerContext;
    readonly IOAuthContext _context;
    readonly IToken _token;

    public ConsumerRequest(IOAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
    {
      _context = context;
      _consumerContext = consumerContext;
      _token = token;
    }

    #region IConsumerRequest Members

    public IOAuthConsumerContext ConsumerContext
    {
      get { return _consumerContext; }
    }

    public IOAuthContext Context
    {
      get { return _context; }
    }

    public XDocument ToDocument()
    {
      return FromStream(stream => XDocument.Load(new StreamReader(stream)));
    }

    public byte[] ToBytes()
    {
      return FromStream(delegate(Stream stream)
                          {
                            var buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            return buffer;
                          });
    }

    public HttpWebRequest ToWebRequest()
    {
      RequestDescription description = GetRequestDescription();

      var request = (HttpWebRequest) WebRequest.Create(description.Url);
      request.Method = description.Method;

      if (description.ContentType == "application/x-www-form-urlencoded")
      {
        request.ContentType = description.ContentType;
        using (var writer = new StreamWriter(request.GetRequestStream()))
        {
          writer.Write(description.Body);
        }
      }

      if (description.Headers.Count > 0)
      {
        foreach (string key in description.Headers.AllKeys)
        {
          request.Headers[key] = description.Headers[key];
        }
      }

      return request;
    }

    public RequestDescription GetRequestDescription()
    {
      if (string.IsNullOrEmpty(_context.Signature))
      {
        if (_token != null)
        {
          _consumerContext.SignContextWithToken(_context, _token);
        }
        else
        {
          _consumerContext.SignContext(_context);
        }
      }

      Uri uri = _context.GenerateUri();

      var description = new RequestDescription
                          {
                            Url = uri,
                            Method = _context.RequestMethod
                          };

      if ((_context.FormEncodedParameters != null) && (_context.FormEncodedParameters.Count > 0))
      {
        description.ContentType = "application/x-www-form-urlencoded";
        description.Body = UriUtility.FormatQueryString(_context.FormEncodedParameters.ToQueryParametersExcludingTokenSecret());
      }

      if (_consumerContext.UseHeaderForOAuthParameters)
      {
        description.Headers[Parameters.OAuth_Authorization_Header] = _context.GenerateOAuthParametersForHeader();
      }

      return description;
    }

    public HttpWebResponse ToWebResponse()
    {
      try
      {
        HttpWebRequest request = ToWebRequest();
        return (HttpWebResponse) request.GetResponse();
      }
      catch (WebException webEx)
      {
        Exception wrappedException;

        if (WebExceptionHelper.TryWrapException(Context, webEx, out wrappedException))
        {
          throw wrappedException;
        }

        throw;
      }
    }

    public NameValueCollection ToBodyParameters()
    {
      try
      {
        string encodedFormParameters = ToString();

        try
        {
          return HttpUtility.ParseQueryString(encodedFormParameters);
        }
        catch (ArgumentNullException)
        {
          throw Error.FailedToParseResponse(encodedFormParameters);
        }
      }
      catch (WebException webEx)
      {
        throw Error.RequestFailed(webEx);
      }
    }

    public IConsumerRequest SignWithoutToken()
    {
      EnsureRequestHasNotBeenSignedYet();
      _consumerContext.SignContext(_context);
      return this;
    }

    public IConsumerRequest SignWithToken()
    {
      return SignWithToken(_token);
    }

    public IConsumerRequest SignWithToken(IToken token)
    {
      EnsureRequestHasNotBeenSignedYet();
      _consumerContext.SignContextWithToken(_context, token);
      return this;
    }

    #endregion

    public override string ToString()
    {
      return FromStream(stream => new StreamReader(stream).ReadToEnd());
    }

    T FromStream<T>(Func<Stream, T> streamParser)
    {
      using (Stream stream = ToWebResponse().GetResponseStream())
      {
        return streamParser(stream);
      }
    }

    void EnsureRequestHasNotBeenSignedYet()
    {
      if (!string.IsNullOrEmpty(_context.Signature))
      {
        throw Error.ThisConsumerRequestHasAlreadyBeenSigned();
      }
    }
  }
}