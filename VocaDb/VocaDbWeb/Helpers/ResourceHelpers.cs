using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace VocaDb.Web.Helpers {

	public static class ResourceHelpers {

		public static MvcHtmlString ToJSON(ResourceManager resourceManager) {

			var dic = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).Cast<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value);

			return new MvcHtmlString(JsonConvert.SerializeObject(dic));

		}

	}

}