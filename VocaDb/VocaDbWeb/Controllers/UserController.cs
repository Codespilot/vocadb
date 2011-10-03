using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class UserController : Controller
    {

		private LoginManager LoginManager {
			get { return MvcApplication.LoginManager; }
		}

		private UserService Service {
			get { return MvcApplication.Services.Users; }
		}

		private UserDetailsContract GetUserDetails() {

			return Service.GetUserDetails(LoginManager.LoggedUser.Id);

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
			var model = Service.GetUserDetails(id);
			return View(model);
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

		public ActionResult MySettings() {

			var user = GetUserDetails();

			return View(new MySettingsModel(user));

		}

		[HttpPost]
		public ActionResult MySettings(MySettingsModel model) {

			var user = LoginManager.LoggedUser;

			if (user.Id != model.Id)
				return new HttpStatusCodeResult(403);

			if (!ModelState.IsValid)
				return View(new MySettingsModel(GetUserDetails()));

			if (!string.IsNullOrEmpty(model.Email)) {
				try {
					new MailAddress(model.Email);
				} catch (FormatException) {
					ModelState.AddModelError("Email", "Invalid email address");
					return View(new MySettingsModel(GetUserDetails()));
				}
			}

			try {
				var newUser = Service.UpdateUserSettings(model.ToContract(), LoginManager);				
				LoginManager.SetLoggedUser(newUser);
			} catch (InvalidPasswordException x) {
				ModelState.AddModelError("OldPass", x.Message);				
			}

			return View(new MySettingsModel(GetUserDetails()));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewAlbum(string newAlbumName) {

			var link = Service.AddAlbum(LoginManager.LoggedUser.Id, newAlbumName);
			return PartialView("AlbumForUserSettingsRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingAlbum(int albumId) {

			var link = Service.AddAlbum(LoginManager.LoggedUser.Id, albumId);
			return PartialView("AlbumForUserSettingsRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteAlbumForUser(int albumForUserId) {

			Service.DeleteAlbumForUser(albumForUserId);

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
