﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Web.Helpers;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models {

	public class RegisterModel {

		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		[RegularExpression("[a-zA-Z0-9_]+")]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "PasswordIsRequired")]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Password")]
		[StringLength(100, MinimumLength = 5)]
		public string Password { get; set; }

	}

	public class LoginModel {

		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "KeepMeLoggedIn")]
		public bool KeepLoggedIn { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "PasswordIsRequired")]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Password")]
		[StringLength(100)]
		public string Password { get; set; }

	}

	public class MySettingsModel {

		public MySettingsModel() {}

		public MySettingsModel(UserForMySettingsContract user) {
			
			ParamIs.NotNull(() => user);

			AnonymousActivity = user.AnonymousActivity;
			CultureSelection = user.Culture;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			EmailOptions = user.EmailOptions;
			Id = user.Id;
			InterfaceLanguageSelection = user.Language;
			PreferredVideoService = user.PreferredVideoService;
			Username = user.Name;

			AccessKey = user.HashedAccessKey;
			AllInterfaceLanguages = InterfaceLanguage.Cultures
				.ToKeyValuePairsWithEmpty(string.Empty, "(Automatic)", c => c.Name, c => c.EnglishName + " (" + c.NativeName + ")")
				.OrderBy(k => k.Value)
				.ToArray();
			AllLanguages = EnumVal<ContentLanguagePreference>.Values.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);
			AllVideoServices = EnumVal<PVService>.Values;

		}

		public string AccessKey { get; set; }

		public KeyValuePair<string, string>[] AllInterfaceLanguages { get; set; }

		public Dictionary<ContentLanguagePreference, string> AllLanguages { get; set; }

		public PVService[] AllVideoServices { get; set; }

		[Display(Name= "Do not show my name in the recent activity list")]
		public bool AnonymousActivity { get; set; }

		public string CultureSelection { get; set; }

		[Display(Name = "Preferred display language")]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[Display(Name = "Email options for private messages")]
		public UserEmailOptions EmailOptions { get; set; }

		[Display(Name = "Interface language")]
		public string InterfaceLanguageSelection { get; set; }

		[Display(Name = "Preferred streaming service")]
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
				Culture = this.CultureSelection ?? string.Empty,
				Id = this.Id,
				Name = Username,
				DefaultLanguageSelection = this.DefaultLanguageSelection,
				Email = this.Email ?? string.Empty,
				EmailOptions = this.EmailOptions,
				Language = this.InterfaceLanguageSelection ?? string.Empty,
				OldPass = this.OldPass,
				PreferredVideoService = this.PreferredVideoService,
				NewPass = this.NewPass
			};

		}

	}

	public class UserEdit {

		public UserEdit() {
			Permissions = new List<PermissionFlagEntry>();
		}

		public UserEdit(UserContract contract) {

			Active = contract.Active;
			GroupId = contract.GroupId;
			Id = contract.Id;
			Name = contract.Name;
			Permissions = PermissionToken.All
				.Select(p => new PermissionFlagEntry(p, contract.AdditionalPermissions.Contains(p), contract.EffectivePermissions.Contains(p))).ToArray();

		}

		public bool Active { get; set; }

		[Display(Name = "User group")]
		public UserGroupId GroupId { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public IList<PermissionFlagEntry> Permissions { get; set; }

		public UserContract ToContract() {

			return new UserContract {
				Active = this.Active,
				GroupId = this.GroupId,
				Id = this.Id,
				Name = this.Name,
				AdditionalPermissions = new HashSet<PermissionToken>(Permissions.Where(p => p.HasFlag).Select(p => PermissionToken.GetById(p.PermissionType.Id)))
			};

		}

	}

	public class PermissionFlagEntry {

		public PermissionFlagEntry() {}

		public PermissionFlagEntry(PermissionToken permissionType, bool hasFlag, bool hasPermission) {
			PermissionType = new PermissionTokenContract(permissionType);
			HasFlag = hasFlag;
			HasPermission = hasPermission;
		}

		public bool HasFlag { get; set; }

		public bool HasPermission { get; set; }

		public PermissionTokenContract PermissionType { get; set; }

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