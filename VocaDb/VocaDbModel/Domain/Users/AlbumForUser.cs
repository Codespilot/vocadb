using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Users {

	public class AlbumForUser {

		private Album album;
		private User user;

		public AlbumForUser() {
			MediaType = MediaType.PhysicalDisc;
			PurchaseStatus = PurchaseStatus.Owned;
		}

		public AlbumForUser(User user, Album album, PurchaseStatus status, MediaType mediaType)
			: this() {

			User = user;
			Album = album;
			PurchaseStatus = status;
			MediaType = mediaType;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual MediaType MediaType { get; set; }

		public virtual PurchaseStatus PurchaseStatus { get; set; }

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual void Delete() {

			Album.UserCollections.Remove(this);
			User.AllAlbums.Remove(this);

		}

		public virtual bool Equals(AlbumForUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as AlbumForUser);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void Move(Album target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Album))
				return;

			Album.UserCollections.Remove(this);
			Album = target;
			target.UserCollections.Add(this);

		}

		public override string ToString() {
			return string.Format("{0} for {1}", Album, User);
		}

	}

}
