using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.EntryValidators;

namespace VocaDb.Model.DataContracts.UseCases {

	public class SongForEditContract : SongDetailsContract {

		public SongForEditContract() {}

		public SongForEditContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference) {
			
			ParamIs.NotNull(() => song);

			Names = song.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			ValidationResult = SongValidator.Validate(song);

		}

		public LocalizedStringWithIdContract[] Names { get; set; }

		public ValidationResult ValidationResult { get; set; }

	}

}
