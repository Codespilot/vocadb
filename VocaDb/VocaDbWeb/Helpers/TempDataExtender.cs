using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers {

	public static class TempDataExtender {

		public static void SetStatusMessage(this TempDataDictionary temp, string val) {
			temp["StatusMessage"] = val;
		}

		public static string StatusMessage(this TempDataDictionary temp) {

			var msg = temp["StatusMessage"];
			return (msg != null ? msg.ToString() : null);

		}

	}

}