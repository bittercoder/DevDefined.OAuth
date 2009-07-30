using System.Collections.Specialized;
using System.Net;
using System.Xml.Linq;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  public interface IConsumerRequest
  {
    IOAuthConsumerContext ConsumerContext { get; }
    IOAuthContext Context { get; }
    XDocument ToDocument();
    byte[] ToBytes();
    HttpWebRequest ToWebRequest();
    HttpWebResponse ToWebResponse();
    NameValueCollection ToBodyParameters();
    RequestDescription GetRequestDescription();
    IConsumerRequest SignWithoutToken();
    IConsumerRequest SignWithToken();
    IConsumerRequest SignWithToken(IToken token);
  }
}