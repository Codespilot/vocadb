using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Mapping.Security {

	public class UserMap : ClassMap<User> {

		public UserMap() {
			
			Schema("dbo");
			Id(m => m.Id);

			Map(m => m.Email).Not.Nullable();
			Map(m => m.Password).Not.Nullable();
			Map(m => m.PermissionFlags).Not.Nullable();
			Map(m => m.Roles).Not.Nullable();
			Map(m => m.Salt).Not.Nullable();

		}

	}

}
