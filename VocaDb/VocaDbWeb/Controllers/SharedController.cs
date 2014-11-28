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

    }
}
