using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeUserRepository : FakeRepository<User>, IUserRepository {

	}

}
