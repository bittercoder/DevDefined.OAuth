using System;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Provider
{
    public class TimestampRangeInspector : IContextInspector
    {
        private readonly Func<DateTime> _nowFunc;
        private TimeSpan _maxAfterNow;
        private TimeSpan _maxBeforeNow;

        public TimestampRangeInspector(TimeSpan window)
            : this(new TimeSpan(window.Ticks/2), new TimeSpan(window.Ticks/2))
        {
        }

        public TimestampRangeInspector(TimeSpan maxBeforeNow, TimeSpan maxAfterNow)
            : this(maxBeforeNow, maxAfterNow, () => DateTime.Now)
        {
        }

        public TimestampRangeInspector(TimeSpan maxBeforeNow, TimeSpan maxAfterNow, Func<DateTime> nowFunc)
        {
            _maxBeforeNow = maxBeforeNow;
            _maxAfterNow = maxAfterNow;
            _nowFunc = nowFunc;
        }

        #region IContextInspector Members

        public void InspectContext(OAuthContext context)
        {
            DateTime timestamp = DateTimeUtility.FromEpoch(Convert.ToInt32(context.Timestamp));
            DateTime now = _nowFunc();

            if (now.Subtract(_maxBeforeNow) > timestamp)
            {
                throw new OAuthException(context, OAuthProblems.TimestampRefused,
                                         string.Format(
                                             "The timestamp is to old, it must be at most {0} seconds before the servers current date and time",
                                             _maxBeforeNow.TotalSeconds));
            }
            if (now.Add(_maxAfterNow) < timestamp)
            {
                throw new OAuthException(context, OAuthProblems.TimestampRefused,
                                         string.Format(
                                             "The timestamp is to far in the future, if must be at most {0} seconds after the server current date and time",
                                             _maxAfterNow.TotalSeconds));
            }
        }

        #endregion
    }
}