using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoTitleParseResult {

		public NicoTitleParseResult(string title)
			: this(title, new string[] {}, SongType.Original) {}

		public NicoTitleParseResult(string title, string[] artistNames, SongType songType) {
			ArtistNames = artistNames;
			Title = title;
			SongType = songType;
		}

		public string[] ArtistNames { get; set; }

		public SongType SongType { get; set; }

		public string Title { get; set; }

	}
}
