using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
  public class DefaultConsumerRequestFactory : IConsumerRequestFactory
  {
    public static readonly DefaultConsumerRequestFactory Instance = new DefaultConsumerRequestFactory();

    public IConsumerRequest CreateConsumerRequest(IOAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
    {
      return new ConsumerRequest(context, consumerContext, token);
    }
  }
}