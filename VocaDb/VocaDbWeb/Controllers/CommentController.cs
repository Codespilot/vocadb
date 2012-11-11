using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Controllers
{
    public class CommentController : ControllerBase
    {

		class EntryComparer : IEqualityComparer<EntryBaseContract> {
			public bool Equals(EntryBaseContract x, EntryBaseContract y) {
				return x.EntryType == y.EntryType && x.Id == y.Id;
			}

			public int GetHashCode(EntryBaseContract obj) {
				return obj.Id;
			}
		}

        //
        // GET: /Comment/

        public ActionResult Index()
        {

			var comments = Services.Admin.GetRecentComments();
			var grouped = comments.GroupBy(c => c.Entry, new EntryComparer());

			return View(grouped);
        }

    }
}
