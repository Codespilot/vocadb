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
    public class MikuDbAlbumController : Controller
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

        	var albums = Service.GetAlbums(status ?? AlbumStatus.New);
			var model = new Index(albums, status ?? AlbumStatus.New);

            return View(model);

        }

		[HttpPost]
		public ActionResult PrepareForImport(IEnumerable<MikuDbAlbumContract> albums) {

			var selectedIds = (albums != null ? albums.Where(a => a.Selected).Select(a => a.Id).ToArray() : new int[] {});
			var result = Service.Inspect(selectedIds);

			return View("PrepareForImport", new PrepareAlbumsForImport(result));

		}

		[HttpPost]
		public ActionResult ImportOne(string AlbumUrl) {
			
			Service.ImportOne(AlbumUrl);

			return RedirectToAction("Index");

		}

		public ActionResult ImportNew() {

			Service.ImportNew();

			return RedirectToAction("Index");

		}

		[HttpPost]
		public ActionResult AcceptImported(IEnumerable<InspectedAlbum> albums) {

			var ids = albums.Select(a => a.ImportedAlbum.Id).ToArray();

			var result = Service.AcceptImportedAlbums(ids);

			return RedirectToAction("Index");

		}

		public ActionResult Delete(int id) {

			Service.Delete(id);
			return RedirectToAction("Index");

		}

		public ActionResult SkipAlbum(int id) {

			Service.SkipAlbum(id);
			return RedirectToAction("Index");

		}

    }
}
