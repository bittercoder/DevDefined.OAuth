namespace DevDefined.OAuth.Core
{
    public interface IToken : IConsumer
    {
        string TokenSecret { get; set; }
        string Token { get; set; }
    }
}