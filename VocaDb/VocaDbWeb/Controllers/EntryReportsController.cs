using System.Web.Mvc;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers
{
    public class EntryReportsController : Controller
    {

		private readonly EntryReportQueries queries;

		public EntryReportsController(EntryReportQueries queries) {
			this.queries = queries;
		}
        
		[Authorize]
		public int NewReportsCount() {

			return queries.GetNewReportsCount();

		}

    }
}
