using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Consumer
{
    public class ClientCertEnabledConsumerRequestFactory : IConsumerRequestFactory
    {
        private readonly ICertificateFactory _certificateFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCertEnabledConsumerRequestFactory"/> class.
        /// </summary>
        /// <param name="certificateFactory">The certificate factory.</param>
        public ClientCertEnabledConsumerRequestFactory(ICertificateFactory certificateFactory)
        {
            _certificateFactory = certificateFactory;
        }

        /// <summary>
        /// Creates the consumer request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="consumerContext">The consumer context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public IConsumerRequest CreateConsumerRequest(IOAuthContext context, IOAuthConsumerContext consumerContext, IToken token)
        {
            return new ClientCertEnabledConsumerRequest(_certificateFactory, context, consumerContext, token);
        }
    }
}