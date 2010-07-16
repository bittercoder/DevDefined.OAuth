using System.Collections.Specialized;

namespace DevDefined.OAuth.Framework
{
    class BoundParameter
    {
        readonly OAuthContext _context;
        readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        public BoundParameter(string name, OAuthContext context)
        {
            _name = name;
            _context = context;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get 
            {
                if (_context.AuthorizationHeaderParameters[_name] != null)
                    return _context.AuthorizationHeaderParameters[_name];

                if (_context.QueryParameters[_name] != null)
                    return _context.QueryParameters[_name];

                if (_context.FormEncodedParameters[_name] != null)
                    return _context.FormEncodedParameters[_name];

                return null;
            }
            set
            {
                if (value == null)
                {
                    Collection.Remove(_name);
                }
                else
                {
                    Collection[_name] = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>The collection.</value>
        NameValueCollection Collection
        {
            get
            {
                if (_context.UseAuthorizationHeader)
                    return _context.AuthorizationHeaderParameters;

                if (_context.RequestMethod == "GET")
                    return _context.QueryParameters;

                return _context.FormEncodedParameters;
            }
        }

    }
}
