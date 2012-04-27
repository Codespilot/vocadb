using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract {

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, 
			IEnumerable<ArtistTagUsage> artists, int artistCount, IEnumerable<AlbumTagUsage> albums, int albumCount,
			IEnumerable<SongTagUsage> songs, int songCount, ContentLanguagePreference languagePreference)
			: base(tag) {

			Albums = albums.Select(a => new AlbumTagUsageContract(a, languagePreference)).ToArray();
			AlbumCount = albumCount;

			Artists = artists.Select(a => new ArtistTagUsageContract(a, languagePreference)).ToArray();
			ArtistCount = artistCount;

			Songs = songs.Select(a => new SongTagUsageContract(a, languagePreference)).ToArray();
			SongCount = songCount;

		}

		public int AlbumCount { get; set; }

		public int ArtistCount { get; set; }

		public AlbumTagUsageContract[] Albums { get; set; }

		public ArtistTagUsageContract[] Artists { get; set; }

		public SongTagUsageContract[] Songs { get; set; }

		public int SongCount { get; set; }

	}

}
