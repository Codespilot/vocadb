using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Service;
using VocaDb.Web.Models.MikuDbAlbums;

namespace VocaDb.Web.Controllers
{
    public class MikuDbAlbumController : ControllerBase
    {

    	private MikuDbAlbumService Service {
    		get { return MvcApplication.Services.MikuDbAlbums; }
    	}

    	public FileResult CoverPicture(int id) {

			var pictureData = Service.GetCoverPicture(id);

			if (pictureData == null)
				return File(Server.MapPath("~/Content/unknown.png"), "image/png");

			return File(pictureData.Bytes, pictureData.Mime);

    	}

        //
        // GET: /MikuDb/

        public ActionResult Index(AlbumStatus? status) {

			var s = status ?? AlbumStatus.New;
        	var albums = Service.GetAlbums(s, 0, entriesPerPage);
			var model = new Index(albums, s);

            return View(model);

        }

		[HttpGet]
		[Authorize]
		public ActionResult PrepareForImport(int id) {

			var result = Service.Inspect(new[] { new ImportedAlbumOptions(id) });

			return View("PrepareForImport", new PrepareAlbumsForImport(result));

		}

		[HttpPost]
		[Authorize]
		public ActionResult PrepareForImport(IEnumerable<MikuDbAlbumContract> albums) {

			var selectedIds = (albums != null ? albums.Where(a => a.Selected).Select(a => new ImportedAlbumOptions(a.Id)).ToArray() : new ImportedAlbumOptions[] {});
			var result = Service.Inspect(selectedIds);

			return View("PrepareForImport", new PrepareAlbumsForImport(result));

		}

		[HttpPost]
		[Authorize]
		public ActionResult ImportFromFile() {

			if (Request.Files.Count == 0)
				return RedirectToAction("Index");

			var file = Request.Files[0];
			var id = Service.ImportFromFile(file.InputStream);

			return RedirectToAction("PrepareForImport", new { id });

		}

		[HttpPost]
		[Authorize]
		public ActionResult ImportOne(string AlbumUrl) {
			
			Service.ImportOne(AlbumUrl);

			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult ImportNew() {

			Service.ImportNew();

			return RedirectToAction("Index");

		}

		[HttpPost]
		[Authorize]
		public ActionResult AcceptImported(IEnumerable<InspectedAlbum> albums, IEnumerable<InspectedTrack> Tracks, string commit) {

			if (commit != "Accept") {

				var options = albums.Select(a => new ImportedAlbumOptions(a)).ToArray();
				var inspectResult = Service.Inspect(options);

				return View("PrepareForImport", new PrepareAlbumsForImport(inspectResult));

			}

			var ids = albums.Select(a => new ImportedAlbumOptions(a)).ToArray();
			var selectedSongIds = (Tracks != null ? Tracks.Where(t => t.Selected).Select(t => t.ExistingSong.Id).ToArray() : new int[] {});

			var result = Service.AcceptImportedAlbums(ids, selectedSongIds);

			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult Delete(int id) {

			Service.Delete(id);
			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult SkipAlbum(int id) {

			Service.SkipAlbum(id);
			return RedirectToAction("Index");

		}

    }
}
