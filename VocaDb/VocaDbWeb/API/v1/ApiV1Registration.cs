using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.API.v1 {

	public class ApiV1Registration : AreaRegistration {

		public override string AreaName {
            get { return "ApiV1"; }
        }
 
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ApiV1",
                "api/v1/{controller}Api/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional });
        }

	}

}