using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract, IEntryWithStatus {

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		EntryType IEntryBase.EntryType {
			get { return EntryType.Tag; }
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, 
			IEnumerable<Artist> artists, int artistCount, IEnumerable<Album> albums, int albumCount,
			IEnumerable<Song> songs, int songCount, ContentLanguagePreference languagePreference)
			: base(tag) {

			Aliases = tag.Aliases.Select(a => a.Name).ToArray();

			Albums = albums.Select(a => new AlbumContract(a, languagePreference)).ToArray();
			AlbumCount = albumCount;

			Artists = artists.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistCount = artistCount;

			Songs = songs.Select(a => new SongContract(a, languagePreference)).ToArray();
			SongCount = songCount;

			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);

		}

		public int AlbumCount { get; set; }

		public int ArtistCount { get; set; }

		public string[] Aliases { get; set; }

		public AlbumContract[] Albums { get; set; }

		public ArtistContract[] Artists { get; set; }

		public SongContract[] Songs { get; set; }

		public int SongCount { get; set; }

		public EntryThumbContract Thumb { get; set; }

	}

}
