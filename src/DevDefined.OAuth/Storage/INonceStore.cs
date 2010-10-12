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

using DevDefined.OAuth.Framework;

namespace DevDefined.OAuth.Storage
{
	/// <summary>
	/// A nonce store is used to avoid requests being "replayed".
	/// </summary>
	public interface INonceStore
	{
		/// <summary>
		/// Will record the none and check if it's unique.
		/// </summary>
		/// <param name="consumer">The consumer associated with the nonce</param>
		/// <param name="nonce">The nonce string itself</param>
		/// <returns><c>true</c> if the nonce is unique, <c>false</c> if the nonce has been presented before</returns>
		bool RecordNonceAndCheckIsUnique(IConsumer consumer, string nonce);
	}
}