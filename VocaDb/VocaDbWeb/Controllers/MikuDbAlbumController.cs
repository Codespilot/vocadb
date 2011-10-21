using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        //
        // GET: /MikuDb/

        public ActionResult Index(AlbumStatus? status) {

        	var albums = Service.GetAlbums(status ?? AlbumStatus.New);
			var model = new Index(albums, status ?? AlbumStatus.New);

            return View(model);

        }

		[HttpPost]
		public ActionResult Index(Index model) {

			var selectedIds = model.Albums.Where(a => a.Selected).Select(a => a.Id).ToArray();

			return RedirectToAction("PrepareForImport", new {ids = selectedIds});

		}

		public ActionResult ImportNew() {

			Service.ImportNew();

			return RedirectToAction("Index");

		}

		public ActionResult PrepareForImport(int[] ids) {

			var result = Service.Inspect(ids);

			return View(result);

		}

    }
}
