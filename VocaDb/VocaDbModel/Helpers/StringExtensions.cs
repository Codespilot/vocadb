namespace VocaDb.Model.Helpers {

	public static class StringExtensions {

		public static string Truncate(this string str, int length) {

			ParamIs.NotNull(() => str);

			return (str.Length > length ? str.Substring(0, length) : str);

		}

		/// <summary>
		/// Truncates a string, adding ellipsis (three dots) at the end if the length exceeds a specific number.
		/// </summary>
		/// <param name="str">String to be processed. Cannot be null.</param>
		/// <param name="length">Maximum length after which the string will be truncated.</param>
		/// <returns>Truncated string with three dots at the end, if the string length exceeded the specified length, otherwise the original string.</returns>
		public static string TruncateWithEllipsis(this string str, int length) {

			ParamIs.NotNull(() => str);

			return (str.Length > length ? str.Substring(0, length) + "..." : str);

		}

	}
}
