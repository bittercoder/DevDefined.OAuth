namespace DevDefined.OAuth.Core
{
    public interface IConsumer
    {
        string ConsumerKey { get; set; }
        string Realm { get; set; }
    }
}