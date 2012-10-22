using System;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class SharedController : Controller
    {

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateEntryPictureFile() {

			return PartialView("EntryPictureFileEditRow", new EntryPictureFileContract());

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateName(string nameVal, ContentLanguageSelection language) {

			return PartialView("LocalizedStringEditableRow", new LocalizedStringEdit { Language = language, Value = nameVal });

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateNewName() {

			return PartialView("LocalizedStringEditableRow", new LocalizedStringEdit());

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateNewWebLink() {

			return PartialView("WebLinkEditRow", new WebLinkDisplay());

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateWebLink(string description, string url, WebLinkCategory category = WebLinkCategory.Other) {

			return PartialView("WebLinkEditRow", new WebLinkDisplay { Description = description, Url = url, Category = category });

		}

		public PartialViewResult Stars(int current, int max) {

			return PartialView(new Tuple<int, int>(current, max));

		}

    }
}
