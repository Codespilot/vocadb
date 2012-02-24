using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserBaseContract {

		public UserBaseContract() { }

		public UserBaseContract(User user) {

			ParamIs.NotNull(() => user);

			Id = user.Id;
			Name = user.Name;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
