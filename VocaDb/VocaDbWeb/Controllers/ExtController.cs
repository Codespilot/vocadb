using System;
using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
    public class ExtController : ControllerBase
    {
        //
        // GET: /Ext/

		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang")]
        public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId) {

			if (songId == invalidId)
				return NoId();

			if (string.IsNullOrEmpty(Request.Params[Model.Service.Security.LoginManager.LangParamName]))
				LoginManager.OverrideLanguage(ContentLanguagePreference.Default);

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult EntryToolTip(string url, string callback) {

			var route = new RouteInfo(new Uri(url), AppConfig.HostAddress).RouteData;
			var controller = route.Values["controller"].ToString();
			var id = int.Parse(route.Values["id"].ToString());
			string data = string.Empty;

			switch (controller) {
				case "Album":
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", Services.Albums.GetAlbum(id));
					break;
				case "Artist":
					data = RenderPartialViewToString("ArtistPopupContent", Services.Artists.GetArtist(id));
					break;
			}

			return Json(data, callback);
		}

    }
}
