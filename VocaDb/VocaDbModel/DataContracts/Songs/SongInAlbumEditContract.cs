using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInAlbumEditContract {

		public SongInAlbumEditContract() { }

		public SongInAlbumEditContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => songInAlbum);

			ArtistString = songInAlbum.Song.ArtistString[languagePreference];
			SongName = songInAlbum.Song.TranslatedName[languagePreference];
			SongAdditionalNames = string.Join(", ", songInAlbum.Song.AllNames.Where(n => n != SongName));
			SongId = songInAlbum.Song.Id;
			SongInAlbumId = songInAlbum.Id;
			TrackNumber = songInAlbum.TrackNumber;

		}

		public SongInAlbumEditContract(string songName) {

			ParamIs.NotNullOrWhiteSpace(() => songName);

			SongName = songName;

		}

		public SongInAlbumEditContract(SongWithAdditionalNamesContract song) {

			ParamIs.NotNull(() => song);

			ArtistString = song.ArtistString;
			SongId = song.Id;
			SongName = song.Name;
			SongAdditionalNames = song.AdditionalNames;

		}

		public string ArtistString { get; set; }

		public string SongAdditionalNames { get; set; }

		public int SongId { get; set; }

		public int SongInAlbumId { get; set; }

		public string SongName { get; set; }

		public int TrackNumber { get; set; }

	}

}
