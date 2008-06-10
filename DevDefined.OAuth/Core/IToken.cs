namespace DevDefined.OAuth.Framework
{
    public interface IToken : IConsumer
    {
        string TokenSecret { get; set; }
        string Token { get; set; }
    }
}