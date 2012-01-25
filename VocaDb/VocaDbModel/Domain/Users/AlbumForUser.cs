﻿using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Users {

	public class AlbumForUser {

		public const int NotRated = 0;

		private Album album;
		private User user;

		public AlbumForUser() {
			MediaType = MediaType.PhysicalDisc;
			Rating = NotRated;
			PurchaseStatus = PurchaseStatus.Owned;
		}

		public AlbumForUser(User user, Album album, PurchaseStatus status, MediaType mediaType, int rating)
			: this() {

			User = user;
			Album = album;
			PurchaseStatus = status;
			MediaType = mediaType;
			Rating = rating;

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

		public virtual int Rating { get; set; }

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
			Album.UpdateRatingTotals();

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
			album.UpdateRatingTotals();
			Album = target;
			target.UserCollections.Add(this);
			target.UpdateRatingTotals();

		}

		public override string ToString() {
			return string.Format("{0} for {1}", Album, User);
		}

	}

}
