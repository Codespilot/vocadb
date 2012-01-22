using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Users {

	public class FavoriteSongForUser {

		private Song song;
		private User user;

		public FavoriteSongForUser() {}

		public FavoriteSongForUser(User user, Song song) {
			User = user;
			Song = song;
		}

		public virtual int Id { get; set; }

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual void Delete() {

			Song.UserFavorites.Remove(this);
			User.FavoriteSongs.Remove(this);
			Song.FavoritedTimes--;

		}

		public virtual bool Equals(FavoriteSongForUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as FavoriteSongForUser);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Song target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.FavoritedTimes--;
			Song.UserFavorites.Remove(this);
			target.FavoritedTimes++;
			target.UserFavorites.Add(this);
			Song = target;

		}

		public override string ToString() {
			return string.Format("favorited {0} for {1}", Song, User);
		}

	}
}
