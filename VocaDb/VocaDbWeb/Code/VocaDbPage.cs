using System.Web.Mvc;

namespace VocaDb.Web.Code {

	public abstract class VocaDbPage<TModel> : WebViewPage<TModel> {

		public string ToJS(bool val) {
			return val ? "true" : "false";
		}

		public string ToJS(int? val) {
			return val.HasValue ? val.ToString() : "null";
		}

	}

}