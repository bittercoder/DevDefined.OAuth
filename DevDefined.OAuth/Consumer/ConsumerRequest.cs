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
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Castle.Core;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
    public class ConsumerRequest
    {
        private readonly IOAuthConsumerContext _consumerContext;
        private readonly OAuthContext _context;
        private readonly IToken _token;

        public ConsumerRequest(OAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
        {
            _context = context;
            _consumerContext = consumerContext;
            _token = token;
        }

        public ConsumerRequest AlterContext(Action<OAuthContext> alteration)
        {
            alteration(_context);
            return this;
        }

        public ConsumerRequest ForMethod(string method)
        {
            _context.RequestMethod = method;
            return this;
        }

        public ConsumerRequest Get()
        {
            return ForMethod("GET");
        }

        public ConsumerRequest Delete()
        {
            return ForMethod("DELETE");
        }

        public ConsumerRequest Put()
        {
            return ForMethod("PUT");
        }

        public ConsumerRequest Post()
        {
            return ForMethod("POST");
        }

        public ConsumerRequest ForUri(Uri uri)
        {
            _context.RawUri = uri;
            return this;
        }

        public ConsumerRequest ForUrl(string url)
        {
            _context.RawUri = new Uri(url);
            return this;
        }

        public ConsumerRequest WithFormParameters(IDictionary dictionary)
        {
            ApplyParameters(_context.FormEncodedParameters, dictionary);
            return this;
        }

        public ConsumerRequest WithFormParameters(object anonymousClass)
        {
            ApplyParameters(_context.FormEncodedParameters, anonymousClass);
            return this;
        }

        public ConsumerRequest WithQueryParameters(IDictionary dictionary)
        {
            ApplyParameters(_context.QueryParameters, dictionary);
            return this;
        }

        public ConsumerRequest WithQueryParameters(object anonymousClass)
        {
            ApplyParameters(_context.QueryParameters, anonymousClass);
            return this;
        }

        public ConsumerRequest WithCookies(IDictionary dictionary)
        {
            ApplyParameters(_context.Cookies, dictionary);
            return this;
        }

        public ConsumerRequest WithCookies(object anonymousClass)
        {
            ApplyParameters(_context.Cookies, anonymousClass);
            return this;
        }

        public ConsumerRequest WithHeaders(IDictionary dictionary)
        {
            ApplyParameters(_context.Headers, dictionary);
            return this;
        }

        public ConsumerRequest WithHeaders(object anonymousClass)
        {
            ApplyParameters(_context.Headers, anonymousClass);
            return this;
        }

        public override string ToString()
        {
            return FromStream(stream => new StreamReader(stream).ReadToEnd());
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

        private T FromStream<T>(Func<Stream, T> streamParser)
        {
            using (Stream stream = ToWebResponse().GetResponseStream())
            {
                return streamParser(stream);
            }
        }

        private void ApplyParameters(NameValueCollection destination, object anonymousClass)
        {
            ApplyParameters(destination, new ReflectionBasedDictionaryAdapter(anonymousClass));
        }

        private void ApplyParameters(NameValueCollection destination, IDictionary additions)
        {
            if (additions == null) throw new ArgumentNullException("additions");

            foreach (string parameter in additions.Keys)
            {
                destination[parameter] = Convert.ToString(additions[parameter]);
            }
        }

        public HttpWebRequest ToWebRequest()
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

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.Method = _context.RequestMethod;

            if ((_context.FormEncodedParameters != null) && (_context.FormEncodedParameters.Count > 0))
            {
                request.ContentType = "application/x-www-form-urlencoded";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(UriUtility.FormatQueryString(_context.FormEncodedParameters));
                }
            }

            if (_consumerContext.UseHeaderForOAuthParameters)
            {
                request.Headers[Parameters.OAuth_Authorization_Header] = _context.GenerateOAuthParametersForHeader();
            }

            return request;
        }

        public HttpWebResponse ToWebResponse()
        {
            HttpWebRequest request = ToWebRequest();
            return (HttpWebResponse) request.GetResponse();
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

        public T Select<T>(Func<NameValueCollection, T> selectFunc)
        {
            try
            {
                return selectFunc(ToBodyParameters());
            }
            catch (ArgumentNullException)
            {
                throw Error.FailedToParseResponse(ToString());
            }
        }

        public ConsumerRequest SignWithoutToken()
        {
            EnsureRequestHasNotBeenSignedYet();
            _consumerContext.SignContext(_context);
            return this;
        }

        public ConsumerRequest SignWithToken()
        {
            return SignWithToken(_token);
        }

        public ConsumerRequest SignWithToken(IToken token)
        {
            EnsureRequestHasNotBeenSignedYet();
            _consumerContext.SignContextWithToken(_context, token);
            return this;
        }

        private void EnsureRequestHasNotBeenSignedYet()
        {
            if (!string.IsNullOrEmpty(_context.Signature))
            {
                throw Error.ThisConsumerRequestHasAlreadyBeenSigned();
            }
        }

        public static implicit operator OAuthContext(ConsumerRequest request)
        {
            return request._context;
        }

        public OAuthContext Context
        {
            get { return this._context; }
        }
    }
}