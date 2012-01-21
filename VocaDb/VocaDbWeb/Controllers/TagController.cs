using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Web.Models.Tag;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase
    {

		private TagService Service {
			get {
				return MvcApplication.Services.Tags;
			}
		}

		public PartialViewResult Albums(string id) {

			return PartialView(Service.GetAlbums(id));

		}

		public PartialViewResult Artists(string id) {

			return PartialView(Service.GetArtists(id));

		}

		public ActionResult Create(string name) {

			if (string.IsNullOrWhiteSpace(name))
				return Json(new GenericResponse<string>(false, "Tag name cannot be empty"));

			name = name.Trim();

			if (!Tag.TagNameRegex.IsMatch(name))
				return Json(new GenericResponse<string>(false, "Tag name may contain only word characters"));

			var view = RenderPartialViewToString("TagSelection", new TagSelectionContract(name, true));

			return Json(new GenericResponse<string>(view));

		}

		public ActionResult Details(string id) {

			var contract = Service.GetTagDetails(id);

			return View(contract);

		}

		public ActionResult Edit(string id) {
			var model = new TagEdit(Service.GetTagForEdit(id));
			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(TagEdit model) {
			
			if (!ModelState.IsValid) {
				var contract = Service.GetTagForEdit(model.Name);
				model.CopyNonEditableProperties(contract);
				return View(model);
			}

			Service.UpdateTag(model.ToContract());

			return RedirectToAction("Details", new { id = model.Name });

		}

		public ActionResult Find(string term) {

			return Json(Service.FindTags(term));

		}

		public ActionResult FindCategories(string term) {

			return Json(Service.FindCategories(term));

		}

		public ActionResult Index() {

			var tags = Service.GetTagsByCategories();

			return View(tags);

		}

		public PartialViewResult Songs(string id) {

			return PartialView(Service.GetSongs(id));

		}

    }
}
