using System.Linq;

namespace VocaDb.Model.Service.VideoServices {

	public static class VideoServiceHelper {

		private static readonly VideoService[] services = new[] { 
			VideoService.NicoNicoDouga,
			VideoService.Youtube
		};

		public static VideoUrlParseResult ParseByUrl(string url) {

			var service = services.FirstOrDefault(s => s.IsValidFor(url));

			if (service == null)
				throw new VideoParseException("No video service defined for URL '" + url + "'");

			return service.ParseByUrl(url);

		}

	}
}
