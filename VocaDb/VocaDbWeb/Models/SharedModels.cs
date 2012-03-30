using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;
using VocaDb.Web.Helpers.Support;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Models {

	public class ObjectCreate {

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

	}

	public class GlobalSearchBoxModel {

		public GlobalSearchBoxModel() {
			AllObjectTypes = new TranslateableEnum<EntryType>(() => Resources.EntryTypeNames.ResourceManager, new[] { EntryType.Artist, EntryType.Album, EntryType.Song });
		}

		public TranslateableEnum<EntryType> AllObjectTypes { get; set; }

		public string GlobalSearchTerm { get; set; }

		public EntryType ObjectType { get; set; }

	}

	public enum SearchObjectType {

		Artist,

		Album,

		Song

	}

	public class LocalizedStringEdit {

		public LocalizedStringEdit() {
			Language = ContentLanguageSelection.Unspecified;
			Value = string.Empty;
		}

		public LocalizedStringEdit(LocalizedStringWithIdContract contract) {

			ParamIs.NotNull(() => contract);

			Id = contract.Id;
			Language = contract.Language;
			Value = contract.Value;

		}

		public int Id { get; set; }

		[Required]
		[Display(Name = "Language")]
		public ContentLanguageSelection Language { get; set; }

		[Required]
		[Display(Name = "Name")]
		public string Value { get; set; }

		public LocalizedStringWithIdContract ToContract() {

			return new LocalizedStringWithIdContract { Id = this.Id, Language = this.Language, Value = this.Value };

		}

	}

	public class AddNewLinkRowModel {

		public AddNewLinkRowModel() {
			FirstColSpan = 1;
		}

		public string EntityName { get; set; }

		public int FirstColSpan { get; set; }

		public string Prefix { get; set; }

		public string SearchUrl { get; set; }

	}

}