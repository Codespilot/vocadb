using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public interface IUserIconFactory {

		string GetIconUrl(User user);

	}

}
