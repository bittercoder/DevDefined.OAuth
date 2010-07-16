using System;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  public class DefaultConsumerRequestFactory : IConsumerRequestFactory
  {
    public static readonly DefaultConsumerRequestFactory Instance = new DefaultConsumerRequestFactory();

    public IConsumerRequest CreateConsumerRequest(IOAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
    {
    	if (context == null) throw new ArgumentNullException("context");
    	if (consumerContext == null) throw new ArgumentNullException("consumerContext");
    	
    	return new ConsumerRequest(context, consumerContext, token);
    }
  }
}