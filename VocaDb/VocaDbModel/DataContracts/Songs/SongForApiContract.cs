using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForApiContract : SongContract {

		public SongForApiContract() { }

		public SongForApiContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference) {

			Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).ToArray();
			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).ToArray();
			Tags = song.Tags.Tags.Select(t => t.Name).ToArray();

		}

		[DataMember]
		public AlbumContract[] Albums { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public string[] Tags { get; set; }

	}
}
