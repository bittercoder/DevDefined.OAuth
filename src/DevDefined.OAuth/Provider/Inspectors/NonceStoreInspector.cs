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
using DevDefined.OAuth.Storage;

namespace DevDefined.OAuth.Provider.Inspectors
{
	public class NonceStoreInspector : IContextInspector
	{
		readonly INonceStore _nonceStore;

		public NonceStoreInspector(INonceStore nonceStore)
		{
			if (nonceStore == null) throw new ArgumentNullException("nonceStore");
			_nonceStore = nonceStore;
		}

		public void InspectContext(ProviderPhase phase, IOAuthContext context)
		{
			if (!_nonceStore.RecordNonceAndCheckIsUnique(context, context.Nonce))
			{
				throw Error.NonceHasAlreadyBeenUsed(context);
			}
		}
	}
}