using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceSoundCloud : VideoService {

		public VideoServiceSoundCloud(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		public override string GetUrlById(string id) {

			var compositeId = new SoundCloudId(id);
			var matcher = linkMatchers.First();
			return "http://" + matcher.MakeLinkFromId(compositeId.SoundCloudUrl);

		}

		public VideoUrlParseResult ParseBySoundCloudUrl(string url) {

			var apiUrl = string.Format("http://api.soundcloud.com/resolve?url=http://soundcloud.com/{0}&client_id=YOUR_CLIENT_ID", url);

			var request = WebRequest.Create(apiUrl);
			XDocument doc;

			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream()) {
				doc = XDocument.Load(stream);
			}

			var trackId = doc.XPathSelectElement("//track/id").Value;
			var title = doc.XPathSelectElement("//track/title").Value;
			var id = new SoundCloudId(trackId, url);

			return new VideoUrlParseResult(PVService.SoundCloud, id.ToString(), title);

		}

		public override VideoUrlParseResult ParseByUrl(string url) {

			var soundCloudUrl = linkMatchers[0].GetId(url);

			return ParseBySoundCloudUrl(soundCloudUrl);

		}

	}

	public class SoundCloudId {

		public SoundCloudId(string trackId, string soundCloudUrl) {

			ParamIs.NotNullOrEmpty(() => trackId);
			ParamIs.NotNullOrEmpty(() => soundCloudUrl);

			TrackId = trackId;
			SoundCloudUrl = soundCloudUrl;

		}

		public SoundCloudId(string compositeId) {

			ParamIs.NotNull(() => compositeId);

			var parts = compositeId.Split(' ');

			if (parts.Length < 2) {
				throw new ArgumentException("Composite ID must contain both track Id and URL");
			}

			TrackId = parts[0];
			SoundCloudUrl = parts[1];

		}

		public string SoundCloudUrl { get; set; }

		public string TrackId { get; set; }

		public override string  ToString() {
			return string.Format("{0} {1}", TrackId, SoundCloudUrl);
		}

	}

}
