using System.Linq;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoService : IVideoService {

		private readonly RegexLinkMatcher[] linkMatchers;
		private readonly IVideoServiceParser parser;

		public VideoService(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) {
			Service = service;
			this.parser = parser;
			this.linkMatchers = linkMatchers;
		}

		public PVService Service { get; private set; }

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

			var matcher = linkMatchers.FirstOrDefault(m => m.IsMatch(url));

			if (matcher == null)
				throw new VideoParseException("No matcher defined for URL '" + url + "'");

			var id = matcher.GetId(url);

			return ParseById(id);

		}

		public VideoUrlParseResult ParseById(string id) {

			var titleResult = GetVideoTitle(id);

			return new VideoUrlParseResult(Service, id, titleResult.Title ?? string.Empty);

		}

	}

}
