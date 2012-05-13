using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public static class VideoServiceHelper {

		private static readonly VideoService[] services = new[] { 
			VideoService.NicoNicoDouga,
			VideoService.Youtube
		};

		public static readonly Dictionary<PVService, VideoService> Services = services.ToDictionary(s => s.Service);

		public static VideoUrlParseResult ParseByUrl(string url) {

			var service = services.FirstOrDefault(s => s.IsValidFor(url));

			if (service == null)
				throw new VideoParseException("No video service defined for URL '" + url + "'");

			return service.ParseByUrl(url);

		}

	}
}
