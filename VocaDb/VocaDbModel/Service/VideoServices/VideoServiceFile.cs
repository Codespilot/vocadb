using System;
using System.Collections.Generic;
using System.Net;
using NLog;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceFile : VideoService {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly HashSet<string> mimeTypes = new HashSet<string>(new[] { "audio/mpeg" }, StringComparer.InvariantCultureIgnoreCase);

		public VideoServiceFile() 
			: base(PVService.File, null, new[] {
				new RegexLinkMatcher("{0}", @"(.+\.mp3$)"),
			}) {}

		public override string GetIdByUrl(string url) {
			return url;
		}

		public override string GetThumbUrlById(string id) {
			return string.Empty;
		}

		public override string GetMaxSizeThumbUrlById(string id) {
			return string.Empty;
		}

		public override string GetUrlById(string id) {
			return id;
		}

		public override VideoTitleParseResult GetVideoTitle(string id) {
			return VideoTitleParseResult.CreateSuccess(string.Empty, string.Empty, string.Empty);
		}

		public override bool IsAuthorized(IUserPermissionContext permissionContext) {
			return permissionContext.HasPermission(PermissionToken.AddRawFileMedia);
		}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {
			return ParseById(url, url, getTitle);
		}

		protected override VideoUrlParseResult ParseById(string id, string url, bool getMeta) {

			var request = WebRequest.CreateHttp(url);
			request.UserAgent = "VocaDB";
			request.Method = "HEAD";
			request.Timeout = 10000;

			try {

				using (var response = request.GetResponse()) {
			
					var mime = response.ContentType;

					if (!mimeTypes.Contains(mime)) {
						return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, string.Format("Unsupported content type: {0}", mime));									
					}

				}

			} catch (WebException x) {
				var msg = string.Format("Unable to load file URL {0}. The file might not be publicly accessible", url);
				log.WarnException(msg, x);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, x));			
			}

			return VideoUrlParseResult.CreateOk(url, PVService.File, id, GetVideoTitle(id));

		}
	}

}
