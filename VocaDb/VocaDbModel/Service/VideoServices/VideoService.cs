using System.Linq;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoService : IVideoService {

		public static VideoService NicoNicoDouga =
			new VideoService(PVService.NicoNicoDouga, new NicoParser(), new[] {
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("www.nicovideo.jp/watch/{0}", @"nicovideo.jp/watch/(\d{6,12})")
			});

		public static VideoService Youtube =
			new VideoService(PVService.Youtube, new YoutubeParser(), new[] {
				new RegexLinkMatcher("youtu.be/{0}", @"youtu.be/(\S{11})"),
				new RegexLinkMatcher("www.youtube.com/watch?v={0}", @"youtube.com/watch?\S*v=(\S{11})"),
			});

		private readonly RegexLinkMatcher[] linkMatchers;
		private readonly IVideoServiceParser parser;

		public VideoService(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) {
			Service = service;
			this.parser = parser;
			this.linkMatchers = linkMatchers;
		}

		public PVService Service { get; private set; }

		public virtual string GetIdByUrl(string url) {

			var matcher = linkMatchers.FirstOrDefault(m => m.IsMatch(url));

			if (matcher == null)
				return null;

			return matcher.GetId(url);

		}

		public virtual string GetUrlById(string id) {

			var matcher = linkMatchers.First();
			return matcher.MakeLinkFromId(id);

		}

		public virtual VideoTitleParseResult GetVideoTitle(string id) {

			return (parser != null ? parser.GetTitle(id) : null);

		}

		public bool IsValidFor(string url) {

			return linkMatchers.Any(m => m.IsMatch(url));

		}

		public bool IsValidFor(PVService service) {
			return (service == Service);
		}

		public VideoUrlParseResult ParseByUrl(string url) {

			var id = GetIdByUrl(url);

			if (id == null)
				throw new VideoParseException("No matcher defined for URL '" + url + "'");

			return ParseById(id);

		}

		public VideoUrlParseResult ParseById(string id) {

			var titleResult = GetVideoTitle(id);

			return new VideoUrlParseResult(Service, id, titleResult.Title ?? string.Empty);

		}

	}

}
