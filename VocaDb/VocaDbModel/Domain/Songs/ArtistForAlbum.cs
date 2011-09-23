using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Songs {
	public class ArtistForAlbum {

		private Artist artist;
		private Album album;

		public ArtistForAlbum() { }

		public ArtistForAlbum(Album album, Artist artist) {
			Album = album;
			Artist = artist;
		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual int Id { get; set; }

	}
}
