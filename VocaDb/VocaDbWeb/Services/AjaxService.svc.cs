using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VocaDb.Web.Services {
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AjaxService" in code, svc and config file together.

	[ServiceContract]
	public class AjaxService  {

		[OperationContract]
		public void UpdateArtistName(int id, string val, string language) {

		}
	}
}
