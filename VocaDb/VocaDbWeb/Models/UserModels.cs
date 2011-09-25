using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Security;

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

		public MySettingsModel(UserContract user) {
			
			ParamIs.NotNull(() => user);

			Email = user.Email;
			Id = user.Id;
			Username = user.Name;

		}

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
				Email = this.Email,
				OldPass = this.OldPass,
				NewPass = this.NewPass
			};

		}

	}

}