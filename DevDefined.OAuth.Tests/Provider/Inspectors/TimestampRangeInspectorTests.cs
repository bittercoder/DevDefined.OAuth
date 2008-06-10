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
﻿using System;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Provider.Inspectors;
using NUnit.Framework;

namespace DevDefined.OAuth.Tests.Provider.Inspectors
{
    [TestFixture]
    public class TimestampRangeInspectorTests
    {
        [Test]
        [ExpectedException(typeof (OAuthException))]
        public void OutsideAfterRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(0, 0, 0), new TimeSpan(1, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 13, 0, 1).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        [ExpectedException(typeof (OAuthException))]
        public void OutsideBeforeRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 10, 59, 59).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        public void WithAfterRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(0, 0, 0), new TimeSpan(1, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 13, 0, 0).Epoch().ToString();

            inspector.InspectContext(context);
        }

        [Test]
        public void WithinBeforeRange()
        {
            var inspector = new TimestampRangeInspector(new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 0),
                                                        () => new DateTime(2008, 1, 1, 12, 0, 0));

            var context = new OAuthContext();
            context.Timestamp = new DateTime(2008, 1, 1, 11, 0, 0).Epoch().ToString();

            inspector.InspectContext(context);
        }
    }
}