using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Web.Controllers;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.API.v1.Controllers {

	public class AlbumApiController : VocaDb.Web.Controllers.ControllerBase {

		private const int maxResults = 25;

		private AlbumService Service {
			get { return Services.Albums; }
		}

		public ActionResult Details(int id) {

			var album = Service.GetAlbumDetails(id);

			return Json(album);

		}

		public ActionResult Index(string query, int? start) {

			var entries = Service.Find(query, start ?? 0, maxResults, false, false);

			return Json(entries);

		}

	}

}