using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase
    {

		private TagService Service {
			get {
				return MvcApplication.Services.Tags;
			}
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

		public ActionResult Find(string term) {

			return Json(Service.FindTags(term));

		}

		public ActionResult Index() {

			var tags = Service.GetTagsByCategories();

			return View(tags);

		}

    }
}
