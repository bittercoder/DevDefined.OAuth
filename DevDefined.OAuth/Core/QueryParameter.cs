namespace DevDefined.OAuth.Core
{
    /// <summary>
    /// An immutable query parameter class.
    /// </summary>
    public class QueryParameter
    {
        public QueryParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }
    }
}