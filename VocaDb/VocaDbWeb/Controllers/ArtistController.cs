using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Service;

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
        public ActionResult Create(ArtistContract artist)
        {
            
			if (string.IsNullOrEmpty(artist.Name)) {
				ModelState.AddModelError("", "Name cannot be empty");
				return RedirectToAction("Index");
			}

        	artist = Service.Create(artist.Name);
        	return RedirectToAction("Details", new {id = artist.Id});

        }
        
        //
        // GET: /Artist/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Artist/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
