using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Users {

	public class ArtistForUser {

		private Artist artist;
		private User user;

		public ArtistForUser() { }

		public ArtistForUser(User user, Artist artist) {
			User = user;
			Artist = artist;
		}

		public virtual int Id { get; set; }

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
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

			User.AllArtists.Remove(this);

		}

		public virtual bool Equals(ArtistForUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ArtistForUser);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist = target;

		}

		public override string ToString() {
			return string.Format("{0} for {1}", Artist, User);
		}

	}

}
