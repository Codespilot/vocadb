using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Deserializes a model attribute using the JSON.NET serializer instead of ASP.NET MVC default serializer.
	/// Note that this is only needed if parts of the form are sended as JSON and the rest is standard form.
	/// </summary>
	public class FromJsonAttribute : CustomModelBinderAttribute {

		public override IModelBinder GetBinder() {
			return new JsonModelBinder();
		}

		private class JsonModelBinder : IModelBinder {
			public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				var stringified = controllerContext.HttpContext.Request[bindingContext.ModelName];
				if (string.IsNullOrEmpty(stringified))
					return null;
				var obj = JsonConvert.DeserializeObject(stringified, bindingContext.ModelType, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				return obj;
			}
		}

	}

	/// <summary>
	/// Deserializes a model attribute using the default ASP.NET serializer.
	/// Note that this is only needed if parts of the form are sended as JSON and the rest is standard form.
	/// </summary>
	/// <remarks>
	/// The JSON.NET implementation should be preferred.
	/// </remarks>
	public class FromJsonMvcAttribute : CustomModelBinderAttribute {
		private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();

		public override IModelBinder GetBinder() {
			return new JsonModelBinder();
		}

		private class JsonModelBinder : IModelBinder {
			public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				var stringified = controllerContext.HttpContext.Request[bindingContext.ModelName];
				if (string.IsNullOrEmpty(stringified))
					return null;
				return serializer.Deserialize(stringified, bindingContext.ModelType);
			}
		}

	}
}