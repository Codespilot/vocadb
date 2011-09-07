using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Service;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class ArtistController : Controller
    {

    	private ArtistService Service {
    		get { return MvcApplication.Services.Artists; }
    	}

        //
        // GET: /Artist/

        public ActionResult Index() {
			ViewBag.Artists = Service.GetArtistsWithAdditionalNames();
            return View();
        }

        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetArtistDetails(id);
            return View(model);
        }

		public ActionResult Picture(int id) {

			var pictureData = Service.GetArtistPicture(id);

			if (pictureData == null)
				return new EmptyResult();

			/*using (var stream = new MemoryStream(pictureData.Bytes)) {
				return new FileStreamResult(stream, pictureData.Mime);
			}*/
			return File(pictureData.Bytes, pictureData.Mime);

		}

        //
        // GET: /Artist/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Artist/Create

        [HttpPost]
        public ActionResult Create(ArtistCreate model)
        {

			if (ModelState.IsValid) {

				var artist = Service.Create(model.Name, MvcApplication.LoginManager);
				return RedirectToAction("Details", new {id = artist.Id});

			}

        	return RedirectToAction("Index", model);

        }
        
        //
        // GET: /Artist/Edit/5
 
        public ActionResult Edit(int id) {
        	var model = new ArtistEdit(Service.GetArtistForEdit(id));
            return View(model);
        }

        //
        // POST: /Artist/Edit/5

        [HttpPost]
		public ActionResult EditBasicDetails(ArtistEdit model)
        {
            try {

            	PictureDataContract pictureData = null;

				if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

					var file = Request.Files[0];
					var buf = new Byte[file.ContentLength];
					file.InputStream.Read(buf, 0, file.ContentLength);
					/*var r = new StreamReader(file.InputStream);
					r.Read()
					var memStream = new MemoryStream();
					file.InputStream.CopyTo(memStream);
					memStream.Write(buf, 0, file.ContentLength);*/

					pictureData = new PictureDataContract(buf, file.ContentType);

				}

				Service.UpdateBasicProperties(model.ToContract(), pictureData, MvcApplication.LoginManager);

				return RedirectToAction("Edit", new { id = model.Id });
            }
            catch {
				return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        //
        // GET: /Artist/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Artist/Delete/5

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
