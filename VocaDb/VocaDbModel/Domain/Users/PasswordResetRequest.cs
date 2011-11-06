using System;

namespace VocaDb.Model.Domain.Users {

	public class PasswordResetRequest {

		private User user;

		public PasswordResetRequest() {
			Created = DateTime.Now;
		}

		public PasswordResetRequest(User user)
			: this() {

			User = user;

		}

		public virtual DateTime Created { get; set; }

		public virtual Guid Id { get; set; }

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value; 
			}
		}
	}
}
