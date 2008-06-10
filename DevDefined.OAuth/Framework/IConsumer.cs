namespace DevDefined.OAuth.Framework
{
    public interface IConsumer
    {
        string ConsumerKey { get; set; }
        string Realm { get; set; }
    }
}