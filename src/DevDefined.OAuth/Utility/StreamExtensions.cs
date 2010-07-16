using System.IO;
using System.Net;

namespace DevDefined.OAuth.Utility
{
  public static class StreamExtensions
  {
    public static string ReadToEnd(this Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new EndOfStreamException("The stream cannot be read");
        }

        if (stream.CanSeek)
        {
            stream.Seek(0, 0);
        }

        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static string ReadToEnd(this WebResponse response)
    {
      return response.GetResponseStream().ReadToEnd();
    }
  }
}