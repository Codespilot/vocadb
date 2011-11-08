using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase
    {

		public ActionResult Create(string name) {

			if (string.IsNullOrWhiteSpace(name))
				return Json(new GenericResponse<string>(false, "Tag name cannot be empty"));

			name = name.Trim();

			if (!Tag.TagNameRegex.IsMatch(name))
				return Json(new GenericResponse<string>(false, "Tag name may contain only word characters"));

			var view = RenderPartialViewToString("TagSelection", new TagSelectionContract(name, true));

			return Json(new GenericResponse<string>(view));

		}

		public string[] Find(string term) {

			return MvcApplication.Services.Other.FindTags(term);

		}

    }
}
