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

			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Names = song.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			ValidationResult = SongValidator.Validate(song);
			UpdateNotes = string.Empty;

		}

		public LyricsForSongContract[] Lyrics { get; set; }

		public LocalizedStringWithIdContract[] Names { get; set; }

		public string UpdateNotes { get; set; }

		public ValidationResult ValidationResult { get; set; }

	}

}
