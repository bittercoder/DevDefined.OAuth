using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevDefined.OAuth.Utility
{
    /// <summary>
    /// DateTime Provider
    /// </summary>
    /// <remarks>Used for testing purposes</remarks>
    public static class DateTimeProvider
    {
        private static Func<DateTime> _now = () => DateTime.Now;

        /// <summary>
        /// Gets or sets the now.
        /// </summary>
        /// <value>The now.</value>
        public static Func<DateTime> Now
        {
            get { return _now; }
            set { _now = value; }
        }



    }
}
