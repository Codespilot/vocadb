using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;

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

			AlbumLinks = user.AlbumLinks;
			AllLanguages = EnumVal<ContentLanguagePreference>.Values;

		}

		public AlbumForUserContract[] AlbumLinks { get; set; }

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

}