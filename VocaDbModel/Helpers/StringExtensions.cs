namespace VocaDb.Model.Helpers {

	public static class StringExtensions {

		public static string Truncate(this string str, int length) {

			ParamIs.NotNull(() => str);

			return (str.Length > length ? str.Substring(0, length) : str);

		}

	}
}
