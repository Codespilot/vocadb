using System.Web.Mvc;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;

namespace VocaDb.Web.API.v1.Controllers {

	public class AlbumApiController : Web.Controllers.ControllerBase {

		private const int maxResults = 25;

		private AlbumService Service {
			get { return Services.Albums; }
		}

		public ActionResult Details(int id) {

			var album = Service.GetAlbumDetails(id);

			return Json(album);

		}

		public ActionResult Index(string query, DiscType discType,
			int start = 0, bool getTotalCount = false, 
			NameMatchMode nameMatchMode = NameMatchMode.Auto) {

			var entries = Service.Find(query, discType, start, maxResults, false, getTotalCount);

			return Json(entries);

		}

	}

}