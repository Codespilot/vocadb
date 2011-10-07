﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

        //
        // GET: /Song/

        public ActionResult Index(string filter, int? page) {

        	var result = Service.Find(filter, ((page ?? 1) - 1)*30, 30);

        	ViewBag.Filter = filter;
        	ViewBag.OnePageOfProducts = new StaticPagedList<SongContract>(result.Items.OrderBy(s => s.Name), page ?? 1, 30, result.TotalCount);

        	return View();

        }

		public ActionResult FindJsonByName(string term) {

			var albums = Service.FindByName(term, 20);

			return Json(albums);

		}

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id) {
        	var model = new SongDetails(Service.GetSongDetails(id));
            return View(model);
        }

        //
        // POST: /Song/Create

        [HttpPost]
        public ActionResult Create(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var song = Service.Create(model.Name);
				return RedirectToAction("Edit", new { id = song.Id });

			} else {

				return RedirectToAction("Index");

			}

        }
        
        //
        // GET: /Song/Edit/5
 
        public ActionResult Edit(int id)
        {
			var model = new SongEdit(Service.GetSongForEdit(id));
			return View(model);
		}

        //
        // POST: /Song/Edit/5

        [HttpPost]
        public ActionResult Edit(SongEdit model) {

			if (!ModelState.IsValid)
				return View();

			var contract = model.ToContract();
			Service.UpdateBasicProperties(contract);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateName(int songId, string nameVal, ContentLanguageSelection language) {

			var name = Service.CreateName(songId, nameVal, language);

			return PartialView("LocalizedStringEditableRow", new LocalizedStringEdit(name));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteName(int nameId) {

			Service.DeleteName(nameId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditNameLanguage(int nameId, string nameLanguage) {

			Service.UpdateNameLanguage(nameId, EnumVal<ContentLanguageSelection>.Parse(nameLanguage));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditNameValue(int nameId, string nameVal) {

			Service.UpdateNameValue(nameId, nameVal);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingArtist(int songId, int artistId) {

			var link = Service.AddArtist(songId, artistId);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewArtist(int songId, string newArtistName) {

			var link = Service.AddArtist(songId, newArtistName);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteArtistForSong(int artistForSongId) {

			Service.DeleteArtistForSong(artistForSongId);

		}

		public PartialViewResult CreateLyrics(int songId, ContentLanguageSelection languageSelection, string source, string value) {
			
			ParamIs.NotNull(() => value);

			var entry = Service.CreateLyrics(songId, languageSelection, value, source);

			return PartialView("LyricsForSongEditRow", entry);

		}

		[HttpPost]
		public ActionResult EditLyrics(SongEdit model, IEnumerable<LyricsForSongModel> lyrics) {

			var contracts = lyrics.Select(l => l.ToContract());
			var song = Service.UpdateLyrics(model.Id, contracts);

			return View("Edit", new SongEdit(song));

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
