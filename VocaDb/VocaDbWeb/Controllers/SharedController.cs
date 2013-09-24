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

		// Refactor: should get rid of these and do with knockout
		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateNewAlias(string nameVal) {

			return PartialView("NameAliasEditRow", new LocalizedStringWithIdContract { Value = nameVal });

		}

		public PartialViewResult Stars(int current = 0, int max = 5) {

			return PartialView(new Tuple<int, int>(current, max));

		}

    }
}
