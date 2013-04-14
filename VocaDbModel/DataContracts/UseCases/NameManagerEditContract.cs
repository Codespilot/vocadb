using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class NameManagerEditContract {

		private LocalizedStringWithIdContract NameOrEmpty(INameManager nameManager, ContentLanguageSelection lang) {

			if (nameManager.HasNameForLanguage(lang))
				return new LocalizedStringWithIdContract(nameManager.FirstNameBase(lang));

			return New(lang);

		}

		private LocalizedStringWithIdContract New(ContentLanguageSelection lang) {
			return new LocalizedStringWithIdContract { Language = lang };
		}

		public NameManagerEditContract() {

			Aliases = new List<LocalizedStringWithIdContract>();
			EnglishName = New(ContentLanguageSelection.English);
			OriginalName = New(ContentLanguageSelection.Japanese);
			RomajiName = New(ContentLanguageSelection.Romaji);

		}

		public NameManagerEditContract(INameManager str)
			: this() {

			ParamIs.NotNull(() => str);

			EnglishName = NameOrEmpty(str, ContentLanguageSelection.English);
			OriginalName = NameOrEmpty(str, ContentLanguageSelection.Japanese);
			RomajiName = NameOrEmpty(str, ContentLanguageSelection.Romaji);

			Aliases = str.NamesBase
				.Where(n => n.Id != EnglishName.Id && n.Id != OriginalName.Id && n.Id != RomajiName.Id)
				.Select(n => new LocalizedStringWithIdContract(n))
				.ToList();

		}

		public List<LocalizedStringWithIdContract> Aliases { get; set; }

		public LocalizedStringWithIdContract EnglishName { get; set; }

		public LocalizedStringWithIdContract OriginalName { get; set; }

		public LocalizedStringWithIdContract RomajiName { get; set; }

		public IEnumerable<LocalizedStringWithIdContract> AllNames {
			get {
				return new[] { OriginalName, RomajiName, EnglishName }.Concat(Aliases).Where(n => !string.IsNullOrEmpty(n.Value)).ToArray();
			}
		}

	}

}