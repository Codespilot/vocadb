using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;

namespace VocaDb.Model.Mapping {

	public class ClassConventions : IClassConvention, IIdConvention, IReferenceConvention, IPropertyConvention, IHasManyConvention {

		private string EscapeColumn(string col) {
			return string.Format("[{0}]", col);
		}

		public void Apply(IClassInstance instance) {
		
			instance.Cache.ReadWrite();
			instance.Schema("dbo");
			instance.Table(instance.EntityType.Name + "s");

		}

		public void Apply(IIdentityInstance instance) {

			instance.Column("Id");
			instance.GeneratedBy.Identity();
	
		}

		public void Apply(IManyToOneInstance instance) {
			instance.Column(EscapeColumn(instance.Name));
		}

		public void Apply(IPropertyInstance instance) {
			instance.Column(EscapeColumn(instance.Name));
		}

		public void Apply(IOneToManyCollectionInstance instance) {
			instance.Key.Column(EscapeColumn(instance.Key.EntityType.Name));
			//instance.LazyLoad(); // TODO: profile this
		}
	}

}
