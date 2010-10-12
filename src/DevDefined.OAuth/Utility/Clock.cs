#region License

// The MIT License
//
// Copyright (c) 2006-2008 DevDefined Limited.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

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
			get { return Now.Epoch().ToString(); }
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