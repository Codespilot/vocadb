using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users {

	public class UserForMySettingsContract : UserContract {

		public UserForMySettingsContract() { }

		public UserForMySettingsContract(User user)
			: base(user) {

			HashedAccessKey = LoginManager.GetHashedAccessKey(user.AccessKey);
			HasPassword = !string.IsNullOrEmpty(user.Password);
			HasTwitterToken = !string.IsNullOrEmpty(user.Options.TwitterOAuthToken);
			TwitterId = user.Options.TwitterId;
			TwitterName = user.Options.TwitterName;

		}

		public string HashedAccessKey { get; set; }

		public bool HasPassword { get; set; }

		public bool HasTwitterToken { get; set; }

		public int TwitterId { get; set; }

		public string TwitterName { get; set; }

	}
}
