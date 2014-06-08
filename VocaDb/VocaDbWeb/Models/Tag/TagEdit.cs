﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models.Tag {

	public class TagEdit {

		public TagEdit() {
			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager).Where(e => e != EntryStatus.Draft).ToArray();
		}

		public TagEdit(TagForEditContract contract)
			: this() {

			ParamIs.NotNull(() => contract);

			AliasedTo = contract.AliasedTo;
			CategoryName = contract.CategoryName;
			Description = contract.Description;
			Name = contract.Name;
			Parent = contract.Parent;
			Status = contract.Status;
			Thumb = contract.Thumb;

			CopyNonEditableProperties(contract);

		}

		public string[] AllCategoryNames { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

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

		public string Parent { get; set; }

		public EntryStatus Status { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public void CopyNonEditableProperties(TagForEditContract contract) {

			AllCategoryNames = contract.AllCategoryNames;
			IsEmpty = contract.IsEmpty;
			Thumb = contract.Thumb;

		}

		public TagContract ToContract() {

			return new TagContract {
				Name = this.Name,
				AliasedTo = this.AliasedTo ?? string.Empty,
				CategoryName = this.CategoryName ?? string.Empty,
				Description = this.Description ?? string.Empty,
				Parent = this.Parent ?? string.Empty,
				Status = this.Status
			};

		}

	}
}