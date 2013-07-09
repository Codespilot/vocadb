using System;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInAlbumEditContract {

		public SongInAlbumEditContract() { }

		public SongInAlbumEditContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInAlbum);

			Artists = songInAlbum.Song.ArtistList.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistString = songInAlbum.Song.ArtistString[languagePreference];
			DiscNumber = songInAlbum.DiscNumber;
			SongName = songInAlbum.Song.TranslatedName[languagePreference];
			SongAdditionalNames = string.Join(", ", songInAlbum.Song.AllNames.Where(n => n != SongName));
			SongId = songInAlbum.Song.Id;
			SongInAlbumId = songInAlbum.Id;
			TrackNumber = songInAlbum.TrackNumber;

		}

		[Obsolete]
		public SongInAlbumEditContract(string songName) {

			ParamIs.NotNullOrWhiteSpace(() => songName);

			Artists = new ArtistContract[0];
			SongName = songName;

		}

		[Obsolete]
		public SongInAlbumEditContract(Song song, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => song);

			Artists = song.ArtistList.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistString = song.ArtistString[languagePreference];
			SongId = song.Id;
			SongName = song.TranslatedName[languagePreference];
			SongAdditionalNames = string.Join(", ", song.AllNames.Where(n => n != SongName));

		}

		public ArtistContract[] Artists { get; set; }

		public string ArtistString { get; set; }

		public int DiscNumber { get; set; }

		public string SongAdditionalNames { get; set; }

		public int SongId { get; set; }

		public int SongInAlbumId { get; set; }

		public string SongName { get; set; }

		public int TrackNumber { get; set; }

	}

}
