using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Albums {

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

		public virtual bool Equals(ArtistForAlbum another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistForAlbum);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Album target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Album.AllArtists.Remove(this);
			Album = target;
			target.AllArtists.Add(this);

		}

		public virtual void Move(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist.AllAlbums.Remove(this);
			Artist = target;
			target.AllAlbums.Add(this);

		}

		public override string ToString() {
			return string.Format("{0} for {1}", Artist, Album);
		}

	}
}
