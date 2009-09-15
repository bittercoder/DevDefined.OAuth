using System.IO;
using System.Net;

namespace DevDefined.OAuth.Utility
{
  public static class StreamExtensions
  {
    public static string ReadToEnd(this Stream stream)
    {
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

    public static string ReadToEnd(this WebResponse response)
    {
      return response.GetResponseStream().ReadToEnd();
    }
  }
}