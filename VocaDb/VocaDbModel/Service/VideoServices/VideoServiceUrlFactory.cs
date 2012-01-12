namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceUrlFactory {

		public static VideoServiceUrlFactory NicoMimi =
			new VideoServiceUrlFactory("http://www.nicomimi.net/play/{0}");

		public static VideoServiceUrlFactory NicoSound = 
			new VideoServiceUrlFactory("http://nicosound.anyap.info/sound/{0}");

		private readonly string template;

		protected VideoServiceUrlFactory(string template) {
			this.template = template;
		}

		public string CreateUrl(string id) {
			return string.Format(template, id);
		}

	}
}
