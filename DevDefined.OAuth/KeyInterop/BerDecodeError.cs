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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace DevDefined.OAuth.KeyInterop
{
  [Serializable]
  public sealed class BerDecodeException : Exception
  {
    readonly int m_position;

    public BerDecodeException()
    {
    }

    public BerDecodeException(String message)
      : base(message)
    {
    }

    public BerDecodeException(String message, Exception ex)
      : base(message, ex)
    {
    }

    public BerDecodeException(String message, int position)
      : base(message)
    {
      m_position = position;
    }

    public BerDecodeException(String message, int position, Exception ex)
      : base(message, ex)
    {
      m_position = position;
    }

    BerDecodeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      m_position = info.GetInt32("Position");
    }

    public int Position
    {
      get { return m_position; }
    }

    public override string Message
    {
      get
      {
        var sb = new StringBuilder(base.Message);

        sb.AppendFormat(" (Position {0}){1}",
                        m_position, Environment.NewLine);

        return sb.ToString();
      }
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Position", m_position);
    }
  }
}