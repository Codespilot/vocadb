using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeTagRepository : FakeRepository<Tag>, ITagRepository {

		protected override ListRepositoryContext<Tag> CreateContext() {
			return new TagListRepositoryContext(querySource);
		}

		public FakeTagRepository(params Tag[] tags)
			: base(tags) {}

	}

	public class TagListRepositoryContext : ListRepositoryContext<Tag> {

		public TagListRepositoryContext(QuerySourceList querySource) 
			: base(querySource) {}

		public override Tag Load(object id) {

			var stringId = (string)id;
			var list = querySource.List<Tag>();
			return list.FirstOrDefault(i => i.Name == stringId);

		}

		public override void Update(Tag obj) {

			var existing = Load(obj.Name);
			Delete(existing);	// Replace existing
			Save(obj);

		}

	}

}
