using System;
using System.Collections.Specialized;

namespace DevDefined.OAuth.Consumer
{
  public class RequestDescription
  {
    public RequestDescription()
    {
      Headers = new NameValueCollection();
    }

    public Uri Url { get; set; }
    public string Method { get; set; }
    public string ContentType { get; set; }
    public string Body { get; set; }
    public NameValueCollection Headers { get; private set; }
  }
}