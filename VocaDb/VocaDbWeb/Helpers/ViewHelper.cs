using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Helpers {

	public static class ViewHelper {

		public static string CreateEmbedHtml(PVService service, string pvId) {

			return null;

		}

		public static string GetVideoServiceLinkName(PVService service, string pvId) {

			return service + " (" + pvId + ")";

		}

	}

}