using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class SongDetailsContract {

		public SongDetailsContract() {}

		public SongDetailsContract(Song song, ContentLanguagePreference languagePreference) {

			Song = new SongContract(song, languagePreference);

			Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).OrderBy(a => a.Name).ToArray();
			AdditionalNames = string.Join(", ", song.AllNames.Where(n => n != Song.Name).Distinct());
			Artists = song.AllArtists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Artist.Name).ToArray();
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			PVs = song.PVs.Select(p => new PVForSongContract(p)).ToArray();
			TranslatedName = new TranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		[DataMember]
		public AlbumContract[] Albums { get; set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public bool IsFavorited { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public PVForSongContract[] PVs { get; set; }

		[DataMember]
		public SongContract Song { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
