using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Service;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : Controller
    {

		private const int entriesPerPage = 50;

		private OtherService Service {
			get { return MvcApplication.Services.Other; }
		}

		public ActionResult EntriesPaged(int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetActivityEntries(pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<ActivityEntryContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), null, "EntriesPaged", "activityEntriesPaged");

			return PartialView("ActivityEntriesPaged", data);

		}

        //
        // GET: /ActivityEntry/

        public ActionResult Index()
        {

			return View();

        }

    }
}
