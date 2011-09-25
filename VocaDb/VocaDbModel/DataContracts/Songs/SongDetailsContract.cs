using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class SongDetailsContract {

		public SongDetailsContract(Song song) {

			Song = new SongContract(song);

			Albums = song.Albums.Select(a => new AlbumContract(a.Album)).ToArray();
			AdditionalNames = string.Join(", ", song.TranslatedName.All.Where(n => n != song.Name));
			Artists = song.AllArtists.Select(a => new ArtistForSongContract(a)).ToArray();
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();

		}

		[DataMember]
		public AlbumContract[] Albums { get; private set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; private set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public SongContract Song { get; private set; }

	}

}
