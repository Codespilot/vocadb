using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.User {

	public class ForgotPassword {

		[Required]
		[Display(Name = "Email address")]
		[StringLength(50)]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string Username { get; set; }

	}

}