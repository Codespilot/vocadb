using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	public class AgentLoginData {

		public AgentLoginData(string name) {
			
			ParamIs.NotNullOrEmpty(() => name);

			Name = name;

		}

		public AgentLoginData(User user) {

			ParamIs.NotNull(() => user);

			Name = user.Name;
			User = user;

		}

		public string Name { get; private set; }

		public User User { get; private set; }

	}

}
