using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models {

	public class RegisterModel {

		[Display(Name = "Email address (optional, but required for password resets)")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[StringLength(100, MinimumLength = 5)]
		public string Password { get; set; }

	}

	public class LoginModel {

		[Display(Name = "Keep me logged in from this computer")]
		public bool KeepLoggedIn { get; set; }

		[Required]
		[Display(Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[StringLength(100)]
		public string Password { get; set; }

	}

	public class MySettingsModel {

		public MySettingsModel() {}

		public MySettingsModel(UserForMySettingsContract user) {
			
			ParamIs.NotNull(() => user);

			AnonymousActivity = user.AnonymousActivity;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			EmailOptions = user.EmailOptions;
			Id = user.Id;
			PreferredVideoService = user.PreferredVideoService;
			Username = user.Name;

			AccessKey = user.HashedAccessKey;
			AlbumLinks = user.AlbumLinks.Select(a => new AlbumForUserEditModel(a)).ToArray();
			AllLanguages = EnumVal<ContentLanguagePreference>.Values.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);
			AllVideoServices = EnumVal<PVService>.Values;

		}

		public string AccessKey { get; set; }

		public AlbumForUserEditModel[] AlbumLinks { get; set; }

		public Dictionary<ContentLanguagePreference, string> AllLanguages { get; set; }

		public PVService[] AllVideoServices { get; set; }

		[Display(Name= "Do not show my name in the recent activity list")]
		public bool AnonymousActivity { get; set; }

		[Display(Name = "Preferred display language")]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[Display(Name = "Email options for private messages")]
		public UserEmailOptions EmailOptions { get; set; }

		[Display(Name = "Preferred video service")]
		public PVService PreferredVideoService { get; set; }

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		public int Id { get; set; }

		[Display(Name = "Username")]
		public string Username { get; set; }

		[Display(Name = "Old password")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string OldPass { get; set; }

		[Display(Name = "New password")]
		[DataType(DataType.Password)]
		[Compare("NewPassAgain", ErrorMessage = "Passwords must match")]
		[StringLength(100)]
		public string NewPass { get; set; }

		[Display(Name = "New password again")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string NewPassAgain { get; set; }

		public UpdateUserSettingsContract ToContract() {

			return new UpdateUserSettingsContract {
				AnonymousActivity = this.AnonymousActivity,
				Id = this.Id,
				Name = Username,
				DefaultLanguageSelection = this.DefaultLanguageSelection,
				Email = this.Email ?? string.Empty,
				EmailOptions = this.EmailOptions,
				OldPass = this.OldPass,
				PreferredVideoService = this.PreferredVideoService,
				NewPass = this.NewPass
			};

		}

	}

	public class UserEdit {

		public UserEdit() {
		}

		public UserEdit(UserContract contract) {

			Active = contract.Active;
			GroupId = contract.GroupId;
			Id = contract.Id;
			Name = contract.Name;
			Permissions = PermissionToken.All
				.Select(p => new PermissionFlagEntry(p, contract.AdditionalPermissions.Has(p))).ToArray();

		}

		public bool Active { get; set; }

		[Display(Name = "User group")]
		public UserGroupId GroupId { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public PermissionFlagEntry[] Permissions { get; set; }

		public UserContract ToContract() {

			return new UserContract {
				Active = this.Active,
				GroupId = this.GroupId,
				Id = this.Id,
				Name = this.Name,
				AdditionalPermissions = new PermissionCollection(Permissions.Where(p => p.HasFlag).Select(p => p.PermissionType))
			};

		}

	}

	public class PermissionFlagEntry {

		public PermissionFlagEntry() {}

		public PermissionFlagEntry(PermissionToken permissionType, bool hasFlag) {
			PermissionType = permissionType;
			HasFlag = hasFlag;
		}

		public bool HasFlag { get; set; }

		public PermissionToken PermissionType { get; set; }

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
			Rating = contract.Rating;

		}

		public AlbumWithAdditionalNamesContract Album { get; set; }

		public MediaType[] AllMediaTypes { get; set; }

		public int Id { get; set; }

		public MediaType MediaType { get; set; }

		public int Rating { get; set; }

	}

}