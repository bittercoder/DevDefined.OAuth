using System;

namespace DevDefined.OAuth.Utility
{
  /// <summary>
  /// A simple class which can be used to generate "unguessable" verifier values.
  /// </summary>
  public class UnguessableGenerator
  {
    const string AllowableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/^()";

    /// <summary>
    /// Generates an unguessable string sequence of a certain length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateUnguessable(int length)
    {
      var random = new Random();

      var chars = new char[length];

      int allowableLength = AllowableCharacters.Length;

      for (int i = 0; i < length; i++)
      {
        chars[i] = AllowableCharacters[random.Next(allowableLength)];
      }

      return new string(chars);
    }

    /// <summary>
    /// Generates an ungessable string, defaults the length to what google uses (24 characters)
    /// </summary>
    /// <returns></returns>
    public static string GenerateUnguessable()
    {
      return GenerateUnguessable(24);
    }
  }
}