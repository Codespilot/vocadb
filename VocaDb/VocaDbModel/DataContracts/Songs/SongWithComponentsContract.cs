using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongWithComponentsContract : SongContract {

		public SongWithComponentsContract() {}

		public SongWithComponentsContract(Song song, ContentLanguagePreference languagePreference, bool includeArtists = false, bool includePVs = false)
			: base(song, languagePreference) {

			if (includeArtists)
				Artists = song.ArtistList.Select(a => new ArtistContract(a, languagePreference)).ToArray();

			if (includePVs)
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();

		}

		[DataMember]
		public ArtistContract[] Artists { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongVoteRating? Vote { get; set; }

	}

}
