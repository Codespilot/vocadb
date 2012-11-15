using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Mapping.Security {

	public class IPRuleMap : ClassMap<IPRule> {

		public IPRuleMap() {

			Id(m => m.Id);

			Map(m => m.Address).Not.Nullable();

		}

	}

}
