using System;

namespace VocaDb.Model.Domain.Security {

	public class User {

		private string email;
		private string password;

		public User() {}

		public User(string email, string pass) {

			Email = email;
			Password = pass;
			PermissionFlags = PermissionFlags.Nothing;
			Salt = new Random().Next();

		}

		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				email = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Password {
			get { return password; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				password = value;
			}
		}

		public virtual PermissionFlags PermissionFlags { get; set; }

		public virtual RoleTypes Roles { get; set; }

		public virtual int Salt { get; set; }

	}

}
