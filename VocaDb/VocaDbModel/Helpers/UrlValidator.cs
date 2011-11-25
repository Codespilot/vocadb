using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Helpers {

	public static class UrlValidator {

		public static bool IsValid(string urlString) {

			ParamIs.NotNullOrWhiteSpace(() => urlString);

			try {
				new Uri(urlString, UriKind.RelativeOrAbsolute);
				return true;
			} catch (UriFormatException) {
				return false;
			}

		}

	}
}
