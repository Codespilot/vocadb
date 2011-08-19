using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class UserController : Controller
    {

		private UserService Service {
			get { return MvcApplication.Services.Users; }
		}

			//
        // GET: /User/

        public ActionResult Index() {
        	ViewBag.Users = Service.GetUsers();
            return View();
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

       public ActionResult Login()
        {
            return View();
        } 

        //
        // POST: /Session/Create

        [HttpPost]
		public ActionResult Login(LoginModel model, string returnUrl)
        {

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.CheckAuthentication(model.UserName, model.Password);

				if (user == null) {
					ModelState.AddModelError("", "Username or password doesn't match");
				} else {

					FormsAuthentication.SetAuthCookie(model.UserName, false);

					return RedirectToAction("Index", "Home");

				}

			}

        	return View(model);

		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Home");
		} 

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(LoginModel model)
        {

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.Create(model.UserName, model.Password);

				if (user == null) {
					ModelState.AddModelError("", "Username is already taken.");
				} else {

					FormsAuthentication.SetAuthCookie(model.UserName, false);

					return RedirectToAction("Index", "Home");

				}

			}

			return View(model);
            
        }
        
        //
        // GET: /User/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /User/Edit/5

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
        // GET: /User/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /User/Delete/5

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
