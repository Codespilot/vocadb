using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Service;

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

			var albums = Service.Find(term, 20).Select(a => a.Name);

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
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Album/Edit/5

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
