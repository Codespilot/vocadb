using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Web.Models.Tag {
	public class TagEdit {

		public TagEdit() { }

		public TagEdit(TagForEditContract contract) {

			ParamIs.NotNull(() => contract);

			CategoryName = contract.CategoryName;
			Description = contract.Description;
			Name = contract.Name;

			CopyNonEditableProperties(contract);

		}

		public string[] AllCategoryNames { get; set; }

		[Display(Name = "Category")]
		public string CategoryName { get; set; }

		[Display(Name = "Description")]
		[StringLength(200)]
		public string Description { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		public void CopyNonEditableProperties(TagForEditContract contract) {

			AllCategoryNames = contract.AllCategoryNames;
			IsEmpty = contract.IsEmpty;

		}

		public TagDetailsContract ToContract() {

			return new TagDetailsContract {
				Name = this.Name,
				CategoryName = this.CategoryName ?? string.Empty,
				Description = this.Description ?? string.Empty
			};

		}

	}
}