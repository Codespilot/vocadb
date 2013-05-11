using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.UseCases {

	/// <summary>
	/// Result of checking a new PV to be posted.
	/// </summary>
	public class NewSongCheckResultContract {

		public NewSongCheckResultContract() {
			Matches = new DuplicateEntryResultContract<SongMatchProperty>[] { };
		}

		public NewSongCheckResultContract(DuplicateEntryResultContract<SongMatchProperty>[] matches, NicoTitleParseResult titleParseResult, ContentLanguagePreference languagePreference) {

			this.Matches = matches;

			if (titleParseResult != null) {
				this.Artists = titleParseResult.Artists.Where(a => a != null).Select(a => new ArtistContract(a, languagePreference)).ToArray();
				this.SongType = titleParseResult.SongType;
				this.Title = titleParseResult.Title;
			}

		}

		public ArtistContract[] Artists { get; set; }

		public DuplicateEntryResultContract<SongMatchProperty>[] Matches { get; set; }

		public SongType SongType { get; set; }

		public string Title { get; set; }

	}
}
