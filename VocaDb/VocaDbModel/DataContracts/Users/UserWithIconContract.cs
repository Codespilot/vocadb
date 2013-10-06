using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// User with profile icon URL.
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserWithIconContract : UserBaseContract, IUserWithIcon {

		public UserWithIconContract() { }

		public UserWithIconContract(User user, string iconUrl)
			: base(user) {

			IconUrl = iconUrl;

		}

		public UserWithIconContract(User user, IUserIconFactory iconFactory)
			: this(user, iconFactory.GetIconUrl(user)) {}

		[DataMember]
		public string IconUrl { get; set; }

	}

}
