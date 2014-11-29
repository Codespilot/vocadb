using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForEditContract : SongDetailsContract {

		public SongForEditContract() {}

		public SongForEditContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference) {
			
			ParamIs.NotNull(() => song);

			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Names = song.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			UpdateNotes = string.Empty;

		}

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

	}

}
