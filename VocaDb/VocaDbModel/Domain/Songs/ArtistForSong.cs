using System;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Songs {

	public class ArtistForSong : IEquatable<ArtistForSong> {

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

		public virtual bool Equals(ArtistForSong another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistForSong);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist.AllSongs.Remove(this);
			Artist = target;
			target.AllSongs.Add(this);

		}

		public virtual void Move(Song target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.AllArtists.Remove(this);
			Song = target;
			target.AllArtists.Add(this);

		}

		public override string ToString() {
			return Artist + " for " + Song;
		}

	}
}
