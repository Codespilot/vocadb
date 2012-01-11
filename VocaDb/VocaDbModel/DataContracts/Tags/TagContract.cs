using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagContract {

		public TagContract() { }

		public TagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			Name = tag.Name;

		}

		public string Name { get; set; }

	}
}
