namespace VocaDb.Model.Domain.Songs {

	public class SongInAlbum {

		private Album album;
		private Song song;

		public SongInAlbum() {}

		public SongInAlbum(Song song, Album album, int trackNumber) {
			Song = song;
			Album = album;
			TrackNumber = trackNumber;
		}

		public virtual int Id { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual int TrackNumber { get; set; }

	}
}
