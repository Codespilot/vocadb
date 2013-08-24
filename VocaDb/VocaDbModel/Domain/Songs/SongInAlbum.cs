using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Songs {

	public class SongInAlbum {

		private Album album;
		private Song song;

		public SongInAlbum() {}

		public SongInAlbum(Song song, Album album, int trackNumber, int discNumber) {
			Song = song;
			Album = album;
			TrackNumber = trackNumber;
			DiscNumber = discNumber;
		}

		public virtual int DiscNumber { get; set; }

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

		public virtual bool Equals(SongInAlbum another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public virtual void Delete() {

			Album.AllSongs.Remove(this);
			Song.AllAlbums.Remove(this);

		}

		public override bool Equals(object obj) {
			return Equals(obj as SongInAlbum);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Album target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Album))
				return;

			Album.AllSongs.Remove(this);
			target.AllSongs.Add(this);
			Album = target;

		}

		public virtual void Move(Song target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.AllAlbums.Remove(this);
			target.AllAlbums.Add(this);
			Song = target;

		}

		public virtual void OnDeleting() {
			
			Album.OnSongDeleting(this);

		}

		public override string ToString() {
			return string.Format("({0}.{1}) {2} in {3}", DiscNumber, TrackNumber, Song, Album);
		}

	}
}
