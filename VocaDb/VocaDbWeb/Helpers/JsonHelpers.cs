using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VocaDb.Web.Helpers {

	public static class JsonHelpers {

		public static string Serialize(object value, bool lowerCase = true) {

			var settings = new JsonSerializerSettings();

			if (lowerCase)
				settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			return JsonConvert.SerializeObject(value, Formatting.None, settings);

		}

	}

}