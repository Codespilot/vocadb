using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class PasswordResetRequestMap : ClassMap<PasswordResetRequest> {

		public PasswordResetRequestMap() {

			Id(m => m.Id).GeneratedBy.GuidComb();

			Map(m => m.Created).Not.Nullable();

			References(m => m.User).Not.Nullable();
		
		}

	}
}
