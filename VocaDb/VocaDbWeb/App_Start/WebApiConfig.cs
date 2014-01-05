using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace VocaDb.Web.App_Start {

	/// <summary>
	/// Configures ASP.NET Web API
	/// </summary>
	public static class WebApiConfig {

		public static void Configure(HttpConfiguration config) {
			
			var json = config.Formatters.JsonFormatter;
			json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // All properties in camel case
			json.SerializerSettings.Converters.Add(new StringEnumConverter());	// All enums as strings by default
			json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				"DefaultApi", "api/{controller}/{id}",
				new { id = RouteParameter.Optional });

		}

	}

}