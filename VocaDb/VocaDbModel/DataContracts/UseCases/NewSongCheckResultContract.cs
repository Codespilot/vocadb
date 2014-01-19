using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
	[DataContract(Namespace = Schemas.VocaDb)]
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
				this.TitleLanguage = titleParseResult.TitleLanguage;
			}

		}

		[DataMember]
		public ArtistContract[] Artists { get; set; }

		[DataMember]
		public DuplicateEntryResultContract<SongMatchProperty>[] Matches { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguageSelection TitleLanguage { get; set; }

	}
}
