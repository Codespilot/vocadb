using System.Web.Mvc;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Controllers
{
    public class ExtController : ControllerBase
    {
        //
        // GET: /Ext/

		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang")]
        public ActionResult EmbedSong(int songId, int pvId = invalidId) {

			if (string.IsNullOrEmpty(Request.Params[Model.Service.Security.LoginManager.LangParamName]))
				LoginManager.OverrideLanguage(ContentLanguagePreference.Default);

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

    }
}
