using System.Linq;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public static class VideoServiceHelper {

		private static readonly VideoService[] services = new[] { 
			new VideoService(PVService.Youtube, new YoutubeParser(), new[] {
				new RegexLinkMatcher("www.youtube.com/watch?v={0}", @"youtube.com/watch?\S*v=(\S{11})"),
				new RegexLinkMatcher("youtu.be/{0}", @"youtu.be/(\S{11})")
			}),
			new VideoService(PVService.NicoNicoDouga, new NicoParser(), new[] {
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/(\d{6,12})")
			})
		};

		public static string GetNicoSoundUrl(string nicoId) {

			return string.Format("http://nicosound.anyap.info/sound/{0}", nicoId);

		}

		public static VideoUrlParseResult ParseByUrl(string url) {

			var service = services.FirstOrDefault(s => s.IsValidFor(url));

			if (service == null)
				throw new VideoParseException("No video service defined for URL '" + url + "'");

			return service.ParseByUrl(url);

		}

	}
}
