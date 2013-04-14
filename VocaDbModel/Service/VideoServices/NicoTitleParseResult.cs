using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoTitleParseResult {

		public NicoTitleParseResult(string title)
			: this(title, new Artist[] { }, SongType.Original) { }

		public NicoTitleParseResult(string title, Artist[] artistNames, SongType songType) {
			ArtistNames = artistNames;
			Title = title;
			SongType = songType;
		}

		public Artist[] ArtistNames { get; set; }

		public SongType SongType { get; set; }

		public string Title { get; set; }

	}
}
