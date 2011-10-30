using System.Linq;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoService : IVideoService {

		private readonly RegexLinkMatcher[] linkMatchers;

		public VideoService(PVService service, RegexLinkMatcher[] linkMatchers) {
			Service = service;
			this.linkMatchers = linkMatchers;
		}

		public PVService Service { get; private set; }

		public virtual string GetVideoTitle(string id) {
			return null;
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

			return new VideoUrlParseResult(Service, id, GetVideoTitle(id));

		}

	}

}
