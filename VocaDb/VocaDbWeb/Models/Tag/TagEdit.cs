using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Web.Models.Tag {
	public class TagEdit {

		public TagEdit() { }

		public TagEdit(TagForEditContract contract) {

			ParamIs.NotNull(() => contract);

			AliasedTo = contract.AliasedTo;
			CategoryName = contract.CategoryName;
			Description = contract.Description;
			Name = contract.Name;

			CopyNonEditableProperties(contract);

		}

		public string[] AllCategoryNames { get; set; }

		[Display(Name = "Aliased to")]
		[StringLength(30)]
		public string AliasedTo { get; set; }

		[Display(Name = "Category")]
		[StringLength(30)]
		public string CategoryName { get; set; }

		[Display(Name = "Description")]
		[StringLength(1000)]
		public string Description { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		public void CopyNonEditableProperties(TagForEditContract contract) {

			AllCategoryNames = contract.AllCategoryNames;
			IsEmpty = contract.IsEmpty;

		}

		public TagContract ToContract() {

			return new TagContract {
				Name = this.Name,
				AliasedTo = this.AliasedTo ?? string.Empty,
				CategoryName = this.CategoryName ?? string.Empty,
				Description = this.Description ?? string.Empty
			};

		}

	}
}