using System.Web.Mvc;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class SharedController : Controller
    {

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateName(string nameVal, ContentLanguageSelection language) {

			return PartialView("LocalizedStringEditableRow", new LocalizedStringEdit { Language = language, Value = nameVal });

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateWebLink(string description, string url) {

			return PartialView("WebLinkEditRow", new WebLinkDisplay { Description = description, Url = url });

		}

    }
}
