using System;

namespace VocaDb.Model.Domain.Security {

	public class User {

		private string email;
		private string name;
		private string nameLc;
		private string password;

		public User() {

			Active = false;
			CreateDate = DateTime.Now;
			Email = string.Empty;
			PermissionFlags = PermissionFlags.Nothing;

		}

		public User(string name, string pass, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Salt = salt;

		}

		public virtual bool Active { get; set; }

		public virtual DateTime CreateDate { get; set; }

		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNull(() => value);
				email = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual DateTime LastLogin { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value;
			}
		}

		public virtual string NameLC {
			get { return nameLc; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				nameLc = value;
			}
		}

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

		public override string ToString() {
			return Name;
		}

		public virtual void UpdateLastLogin() {
			LastLogin = DateTime.Now;
		}

	}

}
