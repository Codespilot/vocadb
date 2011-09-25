using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class AlbumDetailsContract : AlbumContract {

		public AlbumDetailsContract() { }

		public AlbumDetailsContract(Album album)
			: base(album) {

			Artists = album.AllArtists.Select(a => new ArtistContract(a.Artist)).ToArray();
			Songs = album.Songs.Select(s => new SongInAlbumContract(s)).ToArray();

		}

		public ArtistContract[] Artists { get; set; }

		public SongInAlbumContract[] Songs { get; set; }

	}

}
