namespace DevDefined.OAuth.Utility
{
	public static class StringUtility
	{
		public static bool EqualsInConstantTime(this string value, string other)
		{
			if (value == null ^ other == null) return false;
			if (value == null) return true;
			if (value.Length != other.Length) return false;

			return CompareStringsInConstantTime(value, other);
		}

		static bool CompareStringsInConstantTime(string value, string other)
		{
			int result = 0;

			for (int i = 0; i < value.Length; i++)
			{
				result |= value[i] ^ other[i];
			}

			return result == 0;
		}
	}
}