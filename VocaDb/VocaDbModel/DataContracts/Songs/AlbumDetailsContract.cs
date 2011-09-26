using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class AlbumDetailsContract : AlbumContract {

		public AlbumDetailsContract() { }

		public AlbumDetailsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			Artists = album.AllArtists.Select(a => new ArtistContract(a.Artist, languagePreference)).ToArray();
			Description = album.Description;
			Songs = album.Songs.Select(s => new SongInAlbumContract(s)).ToArray();

		}

		public ArtistContract[] Artists { get; set; }

		public string Description { get; set; }

		public SongInAlbumContract[] Songs { get; set; }

	}

}
