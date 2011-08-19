using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Songs {

	public class ArtistForSong {

		private Artist artist;
		private Song song;

		public ArtistForSong() {}

		public ArtistForSong(Song song, Artist artist) {
			Song = song;
			Artist = artist;
		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

	}
}
