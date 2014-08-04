using System;

namespace MonoBrickFirmware.Extensions
{
	/// <summary>
	/// Extensions for strings (some of the code is taken from http://www.dotnetperls.com/between-before-after)
	/// </summary>
	public static class StringEnumExtensions
	{

		/// <summary>
		/// Get string value after [first] a.
		/// </summary>
		public static string Before(this string value, string a)
		{
			int posA = value.IndexOf(a);
			if (posA == -1)
			{
				return "";
			}
			return value.Substring(0, posA);
		}

		/// <summary>
		/// Get string value after [last] a.
		/// </summary>
		public static string After(this string value, string a)
		{
			int posA = value.LastIndexOf(a);
			if (posA == -1)
			{
				return "";
			}
			int adjustedPosA = posA + a.Length;
			if (adjustedPosA >= value.Length)
			{
				return "";
			}
			return value.Substring(adjustedPosA);
		}

	}
}

