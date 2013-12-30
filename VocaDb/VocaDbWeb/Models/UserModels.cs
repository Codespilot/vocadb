﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers.Support;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models {

	public class RegisterModel {

		public RegisterModel() {
			EntryTime = DateTime.Now.Ticks;
		}

		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		/// <summary>
		/// Time when the form was loaded, to track form fill time
		/// </summary>
		public long EntryTime { get; set; }

		/// <summary>
		/// A decoy field for bots
		/// </summary>
		[StringLength(0)]
		public string Extra { get; set; }

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

		public LoginModel() {}

		public LoginModel(string returnUrl) {
			this.ReturnUrl = returnUrl;
		}

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

		public string ReturnUrl { get; set; }

	}

	[PropertyModelBinder]
	public class MySettingsModel {

		public MySettingsModel() {

			AboutMe = string.Empty;
			AllInterfaceLanguages = InterfaceLanguage.Cultures
				.ToKeyValuePairsWithEmpty(string.Empty, ViewRes.User.MySettingsStrings.Automatic, c => c.Name, c => c.NativeName + " (" + c.EnglishName + ")")
				.OrderBy(k => k.Value)
				.ToArray();
			AllLanguages = EnumVal<ContentLanguagePreference>.Values.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);
			AllVideoServices = EnumVal<PVService>.Values;
			Location = string.Empty;
			WebLinks = new List<WebLinkDisplay>();

		}

		public MySettingsModel(UserForMySettingsContract user)
			: this() {
			
			ParamIs.NotNull(() => user);

			AboutMe = user.AboutMe;
			ShowActivity = !user.AnonymousActivity;
			CultureSelection = user.Culture;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			EmailOptions = user.EmailOptions;
			HasPassword = user.HasPassword;
			HasTwitterToken = user.HasTwitterToken;
			Id = user.Id;
			InterfaceLanguageSelection = user.Language;
			Location = user.Location;
			PreferredVideoService = user.PreferredVideoService;
			PublicAlbumCollection = user.PublicAlbumCollection;
			PublicRatings = user.PublicRatings;
			TwitterName = user.TwitterName;
			Username = user.Name;
			WebLinks = user.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			AccessKey = user.HashedAccessKey;

		}

		public string AboutMe { get; set; }

		public string AccessKey { get; set; }

		public KeyValuePair<string, string>[] AllInterfaceLanguages { get; set; }

		public Dictionary<ContentLanguagePreference, string> AllLanguages { get; set; }

		public PVService[] AllVideoServices { get; set; }

		[Display(Name= "Do not show my name in the recent activity list")]
		public bool ShowActivity { get; set; }

		public string CultureSelection { get; set; }

		[Display(Name = "Preferred display language")]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[Display(Name = "Email options for private messages")]
		public UserEmailOptions EmailOptions { get; set; }

		public bool HasPassword { get; set; }

		public bool HasTwitterToken { get; set; }

		[Display(Name = "Interface language")]
		public string InterfaceLanguageSelection { get; set; }

		[StringLength(50)]
		public string Location { get; set; }

		[Display(Name = "Preferred streaming service")]
		public PVService PreferredVideoService { get; set; }

		public bool PublicAlbumCollection { get; set; }

		public bool PublicRatings { get; set; }

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		public int Id { get; set; }

		[Display(Name = "Username")]
		public string Username { get; set; }

		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		[Display(Name = "Old password")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string OldPass { get; set; }

		[Display(Name = "New password")]
		[DataType(DataType.Password)]
		[System.ComponentModel.DataAnnotations.Compare("NewPassAgain", ErrorMessageResourceType = typeof(ViewRes.User.MySettingsStrings), ErrorMessageResourceName = "PasswordsMustMatch")]
		[StringLength(100)]
		public string NewPass { get; set; }

		[Display(Name = "New password again")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string NewPassAgain { get; set; }

		public string TwitterName { get; set; }

		public UpdateUserSettingsContract ToContract() {

			return new UpdateUserSettingsContract {
				AboutMe = this.AboutMe ?? string.Empty,
				AnonymousActivity = !this.ShowActivity,
				Culture = this.CultureSelection ?? string.Empty,
				Id = this.Id,
				Name = Username,
				DefaultLanguageSelection = this.DefaultLanguageSelection,
				Email = this.Email ?? string.Empty,
				EmailOptions = this.EmailOptions,
				Language = this.InterfaceLanguageSelection ?? string.Empty,
				Location = this.Location ?? string.Empty,
				OldPass = this.OldPass,
				PreferredVideoService = this.PreferredVideoService,
				PublicAlbumCollection = this.PublicAlbumCollection,
				PublicRatings = this.PublicRatings,
				NewPass = this.NewPass,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray(),
			};

		}

	}

	public class UserEdit {

		public UserEdit() {

			var groups = EnumVal<UserGroupId>.Values.Where(g => EntryPermissionManager.CanEditGroupTo(Login.Manager, g)).ToArray();
			EditableGroups = new TranslateableEnum<UserGroupId>(() => global::Resources.UserGroupNames.ResourceManager, groups);
			OwnedArtists = new List<ArtistForUserContract>();
			Permissions = new List<PermissionFlagEntry>();

		}

		public UserEdit(UserWithPermissionsContract contract)
			: this() {

			Active = contract.Active;
			GroupId = contract.GroupId;
			Id = contract.Id;
			Name = contract.Name;
			OwnedArtists = contract.OwnedArtistEntries;
			Permissions = PermissionToken.All
				.Select(p => new PermissionFlagEntry(p, contract.AdditionalPermissions.Contains(p), contract.EffectivePermissions.Contains(p))).ToArray();
			Poisoned = contract.Poisoned;

		}

		public bool Active { get; set; }

		public TranslateableEnum<UserGroupId> EditableGroups { get; set; }

		[Display(Name = "User group")]
		public UserGroupId GroupId { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public IList<ArtistForUserContract> OwnedArtists { get; set; }

		public IList<PermissionFlagEntry> Permissions { get; set; }

		public bool Poisoned { get; set; }

		public UserWithPermissionsContract ToContract() {

			return new UserWithPermissionsContract {
				Active = this.Active,
				GroupId = this.GroupId,
				Id = this.Id,
				Name = this.Name,
				OwnedArtistEntries = OwnedArtists.ToArray(),
				Poisoned = this.Poisoned,
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

}