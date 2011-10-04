using System;
using System.Net.Mail;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	public class User {

		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private string email;
		private string name;
		private string nameLc;
		private string password;

		public User() {

			Active = false;
			CreateDate = DateTime.Now;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			LastLogin = DateTime.Now;
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

		public virtual IList<AlbumForUser> Albums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

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

		public virtual AlbumForUser AddAlbum(Album album) {

			ParamIs.NotNull(() => album);

			var link = new AlbumForUser(this, album);
			Albums.Add(link);

			return link;

		}

		public virtual void SetEmail(string newEmail) {
			
			ParamIs.NotNull(() => newEmail);

			if (newEmail != string.Empty)
				new MailAddress(newEmail);

			Email = newEmail;

		}

		public override string ToString() {
			return Name;
		}

		public virtual void UpdateLastLogin() {
			LastLogin = DateTime.Now;
		}

	}

}
