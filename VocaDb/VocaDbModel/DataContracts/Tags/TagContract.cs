using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagContract {

		public TagContract() { }

		public TagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			CategoryName = tag.CategoryName;
			Description = tag.Description;
			Name = tag.Name;

		}

		public string CategoryName { get; set; }

		public string Description { get; set; }

		public string Name { get; set; }

	}
}
