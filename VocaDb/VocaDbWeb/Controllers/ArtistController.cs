using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

				var artist = Service.Create(model.Name);
				return RedirectToAction("Details", new {id = artist.Id});

			}

        	return RedirectToAction("Index", model);

        }
        
        //
        // GET: /Artist/Edit/5
 
        public ActionResult Edit(int id) {
        	var model = new ArtistEdit(Service.GetArtistDetails(id));
            return View(model);
        }

        //
        // POST: /Artist/Edit/5

        [HttpPost]
		public ActionResult EditBasicDetails(ArtistEdit model)
        {
            try
            {
                Service.UpdateBasicProperties(model.ToContract());

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
