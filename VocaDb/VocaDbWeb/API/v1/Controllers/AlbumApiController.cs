using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Controllers;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.API.v1.Controllers {

	public class AlbumApiController : Web.Controllers.ControllerBase {

		private const int maxResults = 10;

		private AlbumService Service {
			get { return Services.Albums; }
		}

		public ActionResult Details(int id = invalidId, DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			if (id == invalidId)
				return NoId();

			var album = Service.GetAlbum(id, a => new AlbumForApiContract(a, lang));

			return Object(album, format);

		}

		public ActionResult Index(string query, DiscType discType = DiscType.Unknown,
			int start = 0, bool getTotalCount = false, AlbumSortRule sort = AlbumSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var queryParams = new AlbumQueryParams(query, discType, start, maxResults, false, getTotalCount, nameMatchMode, sort);

			var entries = Service.Find(a => new AlbumForApiContract(a, lang), queryParams);

			return Object(entries, format);

		}

		public ActionResult Tracks(int id = invalidId, DataFormat format = DataFormat.Auto, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			if (id == invalidId)
				return NoId();

			var tracks = Service.GetAlbum(id, a => a.Songs.Select(s => new SongInAlbumContract(s, lang)).ToArray());

			return Object(tracks, format);

		}

	}

}