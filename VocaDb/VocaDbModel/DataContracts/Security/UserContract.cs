using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	public class UserContract {

		public UserContract(User user) {

			ParamIs.NotNull(() => user);

			Id = user.Id;
			Name = user.Name;

		}

		public int Id { get; private set; }

		public string Name { get; private set; }

	}

}
