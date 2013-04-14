using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserBaseContract : IUser {

		public UserBaseContract() { }

		public UserBaseContract(User user) {

			ParamIs.NotNull(() => user);

			Email = user.Email;
			Id = user.Id;
			Name = user.Name;

		}

		[DataMember]
		public string Email { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
