using System;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Controllers
{
    public class SharedController : Controller
    {

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult CreateEntryPictureFile() {

			return PartialView("EntryPictureFileEditRow", new EntryPictureFileContract());

		}

		public PartialViewResult Stars(int current = 0, int max = 5) {

			return PartialView(new Tuple<int, int>(current, max));

		}

    }
}
