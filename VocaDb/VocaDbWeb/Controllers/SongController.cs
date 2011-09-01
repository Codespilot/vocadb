using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class SongController : Controller
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

        //
        // GET: /Song/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetSongDetails(id);
            return View(model);
        }

        //
        // GET: /Song/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Song/Create

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
        // GET: /Song/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Song/Edit/5

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
        // GET: /Song/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Song/Delete/5

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
