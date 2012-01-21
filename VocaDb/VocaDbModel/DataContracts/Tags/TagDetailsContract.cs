using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract {

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, ContentLanguagePreference languagePreference)
			: base(tag) {

			AlbumCount = tag.AllAlbumTagUsages.Count;
			Albums = tag.AlbumTagUsages.OrderByDescending(a => a.Count).Take(15)
				.Select(a => new AlbumTagUsageContract(a, languagePreference)).ToArray();

			ArtistCount = tag.AllArtistTagUsages.Count;
			Artists = tag.ArtistTagUsages.OrderByDescending(a => a.Count).Take(15)
				.Select(a => new ArtistTagUsageContract(a, languagePreference)).ToArray();

			SongCount = tag.AllSongTagUsages.Count;
			Songs = tag.SongTagUsages.OrderByDescending(a => a.Count).Take(15)
				.Select(a => new SongTagUsageContract(a, languagePreference)).ToArray();

		}

		public int AlbumCount { get; set; }

		public int ArtistCount { get; set; }

		public AlbumTagUsageContract[] Albums { get; set; }

		public ArtistTagUsageContract[] Artists { get; set; }

		public SongTagUsageContract[] Songs { get; set; }

		public int SongCount { get; set; }

	}

}
