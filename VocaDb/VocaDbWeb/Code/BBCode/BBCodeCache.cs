using System.Web;
using VocaDb.Model.Service.BBCode;

namespace VocaDb.Web.Code.BBCode {

	/// <summary>
	/// Utilizes ASP.NET cache to store the results of transformed BBCode.
	/// </summary>
	public class BBCodeCache {

		private readonly BBCodeConverter codeConverter;

		private System.Web.Caching.Cache Cache {
			get {
				return HttpContext.Current.Cache;
			}
		}

		private class CachedValue {

			private readonly string raw;
			private readonly string transformed;

			public CachedValue(string raw, string transformed) {
				this.raw = raw;
				this.transformed = transformed;
			}

			public string Raw {
				get { return raw; }
			}

			public string Transformed {
				get { return transformed; }
			}

		}

		private CachedValue Get(string key) {
			return (CachedValue)Cache[key];
		}

		private void Set(string key, string raw, string transformed) {
			Cache[key] = new CachedValue(raw, transformed);
		}

		public BBCodeCache(BBCodeConverter codeConverter) {
			this.codeConverter = codeConverter;
		}

		public string GetHtml(string rawValue) {
			return GetHtml(rawValue, (rawValue.Length <= 20 ? rawValue : rawValue.GetHashCode().ToString()));
		}

		public string GetHtml(string rawValue, string key) {

			if (string.IsNullOrEmpty(rawValue))
				return rawValue;

			var cached = Get(key);

			if (cached != null && cached.Raw == rawValue)
				return cached.Transformed;

			var transformed = codeConverter.ConvertToHtml(rawValue);

			Set(key, rawValue, transformed);

			return transformed;

		}

	}

}