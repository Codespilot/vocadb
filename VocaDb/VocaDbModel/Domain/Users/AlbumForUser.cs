using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Users {

	public class AlbumForUser {

		private Album album;
		private User user;

		public AlbumForUser() {
			MediaType = MediaType.PhysicalDisc;
		}

		public AlbumForUser(User user, Album album)
			: this() {

			User = user;
			Album = album;

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

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

	}

}
