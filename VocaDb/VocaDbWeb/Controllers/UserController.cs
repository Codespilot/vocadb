using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;
using Microsoft.Web.Helpers;
using MvcPaging;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.User;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
    public class UserController : ControllerBase
    {

		private UserService Service {
			get { return MvcApplication.Services.Users; }
		}

		private UserForMySettingsContract GetUserForMySettings() {

			return Service.GetUserForMySettings(LoginManager.LoggedUser.Id);

		}

		private bool HandleCreate(UserContract user) {

			if (user == null) {
				ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
				return false;
			} else {
				FormsAuthentication.SetAuthCookie(user.Name, false);
				return true;
			}

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void AddArtistForUser(int artistId) {

			Service.AddArtist(LoggedUserId, artistId);

		}

		public ActionResult AlbumCollection(int id, int count, int? page) {

			const int entriesPerPage = 50;
			var pageIndex = (page - 1) ?? 0;
			var albums = Service.GetAlbumCollection(id, PurchaseStatus.Nothing, PagingProperties.CreateFromPage(pageIndex, entriesPerPage, false));
			var paged = new PagingData<AlbumForUserContract>(albums.Items.ToPagedList(pageIndex, entriesPerPage, count), id, "AlbumCollection", "Collection");

			return PartialView(paged);

		}

		public ActionResult Artists(int id) {

			var artists = Service.GetArtists(id);

			return PartialView(artists);

		}

		public void ConnectTwitter() {

			// Make sure session ID is initialized
			// ReSharper disable UnusedVariable
			var sessionId = Session.SessionID;
			// ReSharper restore UnusedVariable

			var twitterSignIn = new TwitterConsumer().TwitterSignIn;

			var uri = new Uri(new Uri(AppConfig.HostAddress), Url.Action("ConnectTwitterComplete"));
			var request = twitterSignIn.PrepareRequestUserAuthorization(uri, null, null);
			var response = twitterSignIn.Channel.PrepareResponse(request);

			response.Send();
			Response.End();

		}

		public ActionResult ConnectTwitterComplete() {

			// Denied authorization
			var param = Request.QueryString["denied"];

			if (!string.IsNullOrEmpty(param)) {
				TempData.SetStatusMessage(ViewRes.User.LoginUsingAuthStrings.SignInCancelled);
				return View("MySettings");
			}

			var response = new TwitterConsumer().ProcessUserAuthorization();

			if (response == null) {
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);
				return View("MySettings");
			}

			int twitterId;
			int.TryParse(response.ExtraData["user_id"], out twitterId);
			var twitterName = response.ExtraData["screen_name"];

			if (Service.ConnectTwitter(response.AccessToken, twitterId, twitterName, WebHelper.GetRealHost(Request))) {
				TempData.SetStatusMessage("Connected successfully");
			} else {
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);
			}

			return RedirectToAction("MySettings");

		}

		public ActionResult EntryEdits(int id, bool? onlySubmissions) {

			var user = Service.GetUserWithActivityEntries(id, 100, onlySubmissions ?? true);

			return View(user);

		}

		[Obsolete]
		public ActionResult FavoriteSongs(int id) {

			return RedirectToAction("FavoriteSongsPaged", new { id });

		}

		public ActionResult FavoriteSongsPaged(int id, int? page) {

			const int songsPerPage = 50;

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetFavoriteSongs(id, PagingProperties.CreateFromPage(pageIndex, songsPerPage, true));
			var data = new PagingData<FavoriteSongForUserContract>(result.Items.ToPagedList(pageIndex, songsPerPage, result.TotalCount), id, "FavoriteSongsPaged", "Favorite_songs");

			return PartialView(data);

		}

		public JsonResult FindByName(string term) {

			var users = Service.FindUsersByName(term).Select(u => u.Name).ToArray();

			return Json(users, "text/json", JsonRequestBehavior.AllowGet);

		}

		public ActionResult ForgotPassword() {

			return View();

		}

		[HttpPost]
		public ActionResult ForgotPassword(ForgotPassword model) {

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"]))
				ModelState.AddModelError("CAPTCHA", "CAPTCHA is invalid.");

			if (!ModelState.IsValid) {
				return View();
			}

			try {
				Service.RequestPasswordReset(model.Username, model.Email, AppConfig.HostAddress + Url.Action("ResetPassword", "User"));
				TempData.SetStatusMessage("Password reset message has been sent.");
				return RedirectToAction("Login");
			} catch (UserNotFoundException) {
				ModelState.AddModelError("Username", "Username or email doesn't match.");
				return View();
			}

		}

			//
        // GET: /User/

        public ActionResult Index(Index model, UserSortRule? sort = null, int? page = null) {

			var pageIndex = (page - 1) ?? 0;
			var groupId = model.GroupId;
			var sortRule = sort ?? UserSortRule.RegisterDate;

        	var users = Service.GetUsers(groupId, sortRule, PagingProperties.CreateFromPage(pageIndex, 300, true));
			return View(new Index(users.Items, groupId));

        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
			var model = Service.GetUserDetails(id);
			return View(model);
		}

		public new ActionResult Profile(string id) {

			var model = Service.GetUserByNameNonSensitive(id);
			return View("Details", model);

		}

       public ActionResult Login()
        {

			RestoreErrorsFromTempData();

            return View();
        } 

        //
        // POST: /Session/Create

        [HttpPost]
		public ActionResult Login(LoginModel model, string returnUrl)
        {

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.CheckAuthentication(model.UserName, model.Password, CfHelper.GetRealIp(Request));

				if (user == null) {
					ModelState.AddModelError("", "Username or password doesn't match");
					//TempData.SetErrorMessage("Username or password doesn't match");
				} else {

					TempData.SetSuccessMessage(string.Format(ViewRes.User.LoginStrings.Welcome, user.Name));
					FormsAuthentication.SetAuthCookie(model.UserName, model.KeepLoggedIn);

					var redirectUrl = FormsAuthentication.GetRedirectUrl(model.UserName, true);

					if (redirectUrl != null)
						return Redirect(redirectUrl);
					else
						return RedirectToAction("Index", "Home");

				}

			}

        	return View(model);

		}

		public void LoginTwitter() {
			
			// Make sure session ID is initialized
// ReSharper disable UnusedVariable
			var sessionId = Session.SessionID;
// ReSharper restore UnusedVariable

			var twitterSignIn = new TwitterConsumer().TwitterSignIn;

			var uri = new Uri(new Uri(AppConfig.HostAddress), Url.Action("LoginTwitterComplete"));
			var request = twitterSignIn.PrepareRequestUserAuthorization(uri, null, null);
			var response = twitterSignIn.Channel.PrepareResponse(request);

			response.Send();
			Response.End();

		}

		public ActionResult LoginTwitterComplete() {

			// Denied authorization
			var param = Request.QueryString["denied"];

			if (!string.IsNullOrEmpty(param)) {
				TempData.SetStatusMessage(ViewRes.User.LoginUsingAuthStrings.SignInCancelled);
				return View("Login");
			}

			var response = new TwitterConsumer().ProcessUserAuthorization();

			if (response == null) {
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);
				return View("Login");
			}

			var user = Service.CheckTwitterAuthentication(response.AccessToken, Hostname);

			if (user == null) {
				int twitterId;
				int.TryParse(response.ExtraData["user_id"], out twitterId);
				var twitterName = response.ExtraData["screen_name"];
				return View(new RegisterOpenAuthModel(response.AccessToken, twitterName, twitterId, twitterName));
			}

			HandleCreate(user);

			return RedirectToAction("Index", "Home");

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LoginTwitterComplete(RegisterOpenAuthModel model) {

			if (ModelState.IsValid) {

				var user = Service.CreateTwitter(model.AccessToken, model.UserName, model.Email ?? string.Empty, 
					model.TwitterId, model.TwitterName, Hostname);

				if (HandleCreate(user))
					return RedirectToAction("Index", "Home");

			}

			return View(model);

		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult ComposeMessage(ComposeMessage model) {

			if (!ModelState.IsValid) {
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			var contract = model.ToContract(LoggedUserId);

			try {
				Service.SendMessage(contract, AppConfig.HostAddress + Url.Action("Messages", "User"));
			} catch (UserNotFoundException x) {
				ModelState.AddModelError("ReceiverName", x.Message);
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			TempData.SetStatusMessage(ViewRes.User.MessagesStrings.MessageSent);

			return RedirectToAction("Messages");

		}

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View(new RegisterModel());
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(RegisterModel model)
        {

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"]))
				ModelState.AddModelError("CAPTCHA", ViewRes.User.CreateStrings.CaptchaInvalid);

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.Create(model.UserName, model.Password, model.Email ?? string.Empty, Hostname);

				if (HandleCreate(user))
					return RedirectToAction("Index", "Home");

			}

			return View(model);
            
        }

		[HttpPost]
		public PartialViewResult CreateComment(int entryId, string message) {

			var comment = Service.CreateComment(entryId, message);

			return PartialView("Comment", comment);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

        //
        // GET: /User/Edit/5

		[Authorize]
		public ActionResult Edit(int id)
        {

			LoginManager.VerifyPermission(PermissionToken.ManageUserPermissions);

        	var user = Service.GetUser(id);
            return View(new UserEdit(user));

        }

        //
        // POST: /User/Edit/5
		[Authorize]
        [HttpPost]
		public ActionResult Edit(UserEdit model, IEnumerable<PermissionFlagEntry> permissions) {

			LoginManager.VerifyPermission(PermissionToken.ManageUserPermissions);

			if (permissions != null)
				model.Permissions = permissions.ToArray();

			Service.UpdateUser(model.ToContract());

        	return RedirectToAction("Details", new {id = model.Id});

        }

		public PartialViewResult Message(int messageId) {

			return PartialView("Message", Service.GetMessageDetails(messageId));

		}

		[Authorize]
		public ActionResult Messages() {

			var user = Service.GetUserWithMessages(LoggedUserId);
			RestoreErrorsFromTempData();

			return View(user);

		}

		[Authorize]
		public ActionResult MySettings() {

			LoginManager.VerifyPermission(PermissionToken.EditProfile);			

			var user = GetUserForMySettings();

			return View(new MySettingsModel(user));

		}

		[HttpPost]
		public ActionResult MySettings(MySettingsModel model) {

			var user = LoginManager.LoggedUser;

			if (user.Id != model.Id)
				return new HttpStatusCodeResult(403);

			if (!ModelState.IsValid)
				return View(new MySettingsModel(GetUserForMySettings()));

			if (!string.IsNullOrEmpty(model.Email)) {
				try {
					new MailAddress(model.Email);
				} catch (FormatException) {
					ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
					return View(model);
				}
			}

			try {
				var newUser = Service.UpdateUserSettings(model.ToContract());				
				LoginManager.SetLoggedUser(newUser);
			} catch (InvalidPasswordException x) {
				ModelState.AddModelError("OldPass", x.Message);
				return View(model);
			}

			TempData.SetStatusMessage(ViewRes.User.MySettingsStrings.SettingsUpdated);

			return RedirectToAction("Profile", new { id = user.Name });

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void RemoveAlbumFromUser(int albumId) {
			
			Service.RemoveAlbumFromUser(LoggedUserId, albumId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void RemoveArtistFromUser(int artistId) {

			Service.RemoveArtistFromUser(LoggedUserId, artistId);

		}

		public ActionResult ResetAccesskey() {

			Service.ResetAccessKey();
			TempData.SetStatusMessage("Access key reset");
			return RedirectToAction("MySettings");

		}

		public ActionResult ResetPassword(Guid id) {

			var model = new ResetPassword();

			if (!Service.CheckPasswordResetRequest(id)) {
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			} else {
				model.RequestId = id;
			}

			return View(model);

		}

		[HttpPost]
		public ActionResult ResetPassword(ResetPassword model) {

			if (!Service.CheckPasswordResetRequest(model.RequestId)) {
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			}

			if (!ModelState.IsValid) {
				return View(new ResetPassword());
			}

			var user = Service.ResetPassword(model.RequestId, model.NewPass);
			FormsAuthentication.SetAuthCookie(user.Name, false);

			TempData.SetStatusMessage("Password reset successfully!");

			return RedirectToAction("Index", "Home");

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateAlbumForUser(int albumid, PurchaseStatus collectionStatus, MediaType mediaType, int rating) {

			Service.UpdateAlbumForUser(LoggedUserId, albumid, collectionStatus, mediaType, rating);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete]
		public void UpdateAlbumForUserMediaType(int albumForUserId, MediaType mediaType) {
			
			Service.UpdateAlbumForUserMediaType(albumForUserId, mediaType);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete]
		public void UpdateAlbumForUserRating(int albumForUserId, int rating) {

			Service.UpdateAlbumForUserRating(albumForUserId, rating);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete]
		public PartialViewResult AddExistingAlbum(int albumId) {

			var link = Service.AddAlbum(LoggedUserId, albumId);
			return PartialView("AlbumForUserSettingsRow", new AlbumForUserEditModel(link));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteAlbumForUser(int albumForUserId) {

			Service.DeleteAlbumForUser(albumForUserId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void AddSongToFavorites(int songId, SongVoteRating rating = SongVoteRating.Favorite) {

			Service.UpdateSongRating(LoggedUserId, songId, rating);

		}

		public ActionResult Disable(int id) {

			Service.DisableUser(id);

			return RedirectToAction("Details", new { id });

		}

    }
}
