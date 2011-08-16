using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	public class UserContract {

		public UserContract(User user) {
			Id = user.Id;
		}

		public int Id { get; set; }

	}

}
