using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.BBCode {

	public class DefaultBBCodeConverter {

		public static BBCodeConverter Create() {

			return new BBCodeConverter(new[] { new AutoLinkTransformer() });

		}

	}

}