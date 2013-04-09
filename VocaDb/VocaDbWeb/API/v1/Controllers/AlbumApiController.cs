using System.Web.Mvc;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Web.Controllers;

namespace VocaDb.Web.API.v1.Controllers {

	public class AlbumApiController : Web.Controllers.ControllerBase {

		private const int maxResults = 25;

		private AlbumService Service {
			get { return Services.Albums; }
		}

		public ActionResult Details(int id, DataFormat format = DataFormat.Auto) {

			var album = Service.GetAlbumDetails(id, null);

			return Object(album, format);

		}

		public ActionResult Index(string query, DiscType discType = DiscType.Unknown,
			int start = 0, bool getTotalCount = false, AlbumSortRule sort = AlbumSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, DataFormat format = DataFormat.Auto) {

			var entries = Service.Find(query, discType, start, maxResults, false, getTotalCount, nameMatchMode, sort);

			return Object(entries, format);

		}

	}

}