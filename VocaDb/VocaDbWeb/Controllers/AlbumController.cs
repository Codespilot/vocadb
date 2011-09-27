using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Service;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private AlbumService Service {
			get { return MvcApplication.Services.Albums; }
		}

        //
        // GET: /Album/

        public ActionResult Index() {
        	ViewBag.Albums = Service.GetAlbums();
            return View();
        }

		public ActionResult FindJson(string term) {

			var albums = Service.Find(term, 20);

			return Json(albums);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetAlbumDetails(id);
            return View(model);
        }

        //
        // GET: /Album/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Album/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Album/Edit/5
 
        public ActionResult Edit(int id) {

        	var album = Service.GetAlbumForEdit(id);

			return View(new AlbumEdit(album));

        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
		public ActionResult Edit(AlbumEdit model)
        {

			if (!ModelState.IsValid)
				return View();

            PictureDataContract pictureData = null;

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				var file = Request.Files[0];
				var buf = new Byte[file.ContentLength];
				file.InputStream.Read(buf, 0, file.ContentLength);

				pictureData = new PictureDataContract(buf, file.ContentType);

			}

            var contract = model.ToContract();
			Service.UpdateBasicProperties(contract, pictureData);

        	return RedirectToAction("Details", new { id = model.Id });

        }

        //
        // GET: /Album/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Album/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
