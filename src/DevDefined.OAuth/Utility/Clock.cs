using System;
using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Utility
{
	public static class Clock
	{
		static Func<DateTime> _nowFunc;

		static Clock()
		{
			_nowFunc = () => DateTime.Now;
		}

		public static string EpochString
		{
			get
			{
				return Now.Epoch().ToString();
			}
		}

		public static DateTime Now
		{
			get { return _nowFunc(); }
		}

		public static IDisposable FreezeAt(DateTime time)
		{
			return ReplaceImplementation(() => time);
		}

		public static IDisposable Freeze()
		{
			DateTime now = Now;
			return ReplaceImplementation(() => now);
		}

		public static void Reset()
		{
			_nowFunc = () => DateTime.Now;
		}

		public static IDisposable ReplaceImplementation(Func<DateTime> nowFunc)
		{
			Func<DateTime> originalFunc = _nowFunc;
			_nowFunc = nowFunc;
			return new DisposableAction(() => _nowFunc = originalFunc);
		}
	}
}