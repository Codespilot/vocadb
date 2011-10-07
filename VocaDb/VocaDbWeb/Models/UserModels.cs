using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models {

	public class LoginModel {

		[Required]
		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

	}

	public class MySettingsModel {

		public MySettingsModel() {}

		public MySettingsModel(UserDetailsContract user) {
			
			ParamIs.NotNull(() => user);

			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			Id = user.Id;
			Username = user.Name;

			AlbumLinks = user.AlbumLinks.Select(a => new AlbumForUserEditModel(a)).ToArray();
			AllLanguages = EnumVal<ContentLanguagePreference>.Values;

		}

		public AlbumForUserEditModel[] AlbumLinks { get; set; }

		public ContentLanguagePreference[] AllLanguages { get; set; }

		[Display(Name = "Preferred display language")]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		public int Id { get; set; }

		[Display(Name = "Username")]
		public string Username { get; set; }

		[Display(Name = "Old password")]
		[DataType(DataType.Password)]
		public string OldPass { get; set; }

		[Display(Name = "New password")]
		[DataType(DataType.Password)]
		[Compare("NewPassAgain", ErrorMessage = "Passwords must match")]
		public string NewPass { get; set; }

		[Display(Name = "New password again")]
		[DataType(DataType.Password)]
		public string NewPassAgain { get; set; }

		public UpdateUserSettingsContract ToContract() {

			return new UpdateUserSettingsContract {
				Id = this.Id,
				Name = Username,
				DefaultLanguageSelection = this.DefaultLanguageSelection,
				Email = this.Email,
				OldPass = this.OldPass,
				NewPass = this.NewPass
			};

		}

	}

	public class UserEdit {

		public UserEdit() {
		}

		public UserEdit(UserContract contract) {

			Id = contract.Id;
			Name = contract.Name;
			Permissions = EnumVal<PermissionFlags>.Values.Where(p => p != PermissionFlags.Nothing && p != PermissionFlags.Default)
				.Select(p => new PermissionFlagEntry(p, contract.PermissionFlags.HasFlag(p))).ToArray();

		}

		public int Id { get; set; }

		public string Name { get; set; }

		public PermissionFlagEntry[] Permissions { get; set; }

		public UserContract ToContract() {

			return new UserContract {
				Id = this.Id,
				Name = this.Name,
				PermissionFlags =
					Permissions.Aggregate(PermissionFlags.Nothing,
						(flags, item) => flags | (item.HasFlag ? item.PermissionType : PermissionFlags.Nothing))
			};

		}

	}

	public class PermissionFlagEntry {

		public PermissionFlagEntry() {}

		public PermissionFlagEntry(PermissionFlags permissionType, bool hasFlag) {
			PermissionType = permissionType;
			HasFlag = hasFlag;
		}

		public bool HasFlag { get; set; }

		public PermissionFlags PermissionType { get; set; }

	}

	public class AlbumForUserEditModel {

		public AlbumForUserEditModel() {
			AllMediaTypes = EnumVal<MediaType>.Values;
		}

		public AlbumForUserEditModel(AlbumForUserContract contract)
			: this() {

			Album = contract.Album;
			Id = contract.Id;
			MediaType = contract.MediaType;

		}

		public AlbumDetailsContract Album { get; set; }

		public MediaType[] AllMediaTypes { get; set; }

		public int Id { get; set; }

		public MediaType MediaType { get; set; }


	}

}