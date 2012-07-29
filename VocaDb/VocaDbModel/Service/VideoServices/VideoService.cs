using System.Linq;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoService : IVideoService {

		public static readonly VideoService NicoNicoDouga =
			new VideoServiceNND(PVService.NicoNicoDouga, new NicoParser(), new[] {
				new RegexLinkMatcher("nico.ms/{0}", @"nicovideo.jp/watch/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("nico.ms/{0}", @"nicovideo.jp/watch/(\d{6,12})"),
				new RegexLinkMatcher("nico.ms/{0}", @"nico.ms/([a-z]{2}\d{4,10})"),
				new RegexLinkMatcher("nico.ms/{0}", @"nico.ms/(\d{6,12})")
			});

		public static readonly VideoService SoundCloud =
			new VideoServiceSoundCloud(PVService.SoundCloud, null, new[] {
				new RegexLinkMatcher("soundcloud.com/{0}", @"soundcloud.com/(\S+)"),
			});

		public static readonly VideoService Youtube =
			new VideoServiceYoutube(PVService.Youtube, new YoutubeParser(), new[] {
				new RegexLinkMatcher("youtu.be/{0}", @"youtu.be/(\S{11})"),
				new RegexLinkMatcher("www.youtube.com/watch?v={0}", @"youtube.com/watch?\S*v=(\S{11})"),
			});

		protected readonly RegexLinkMatcher[] linkMatchers;
		private readonly IVideoServiceParser parser;

		protected VideoService(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) {
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

		public virtual string GetThumbUrlById(string id) {

			return null;

		}

		public virtual string GetUrlById(string id) {

			var matcher = linkMatchers.First();
			return string.Format("http://{0}", matcher.MakeLinkFromId(id));

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

		public virtual VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			var id = GetIdByUrl(url);

			if (id == null) {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher);
			}

			return ParseById(id, url, getTitle);

		}

		public virtual VideoUrlParseResult ParseById(string id, string url, bool getTitle) {

			var title = (getTitle ? GetVideoTitle(id).Title ?? string.Empty : string.Empty);

			return VideoUrlParseResult.CreateOk(url, Service, id, title);

		}

	}

}
