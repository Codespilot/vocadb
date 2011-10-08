using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	public class FrontPageContract {

		public FrontPageContract(IEnumerable<Album> latestAlbums, IEnumerable<Song> latestSongs, 
			ContentLanguagePreference languagePreference) {

			LatestAlbums = latestAlbums.Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).ToArray();
			LatestSongs = latestSongs.Select(s => new SongWithAdditionalNamesContract(s, languagePreference)).ToArray();

		}

		public AlbumWithAdditionalNamesContract[] LatestAlbums { get; set; }

		public SongWithAdditionalNamesContract[] LatestSongs { get; set; }

	}
}
