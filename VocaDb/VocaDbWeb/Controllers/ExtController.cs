using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class ExtController : ControllerBase
    {
        //
        // GET: /Ext/

        public ActionResult EmbedSong(int songId) {

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

    }
}
