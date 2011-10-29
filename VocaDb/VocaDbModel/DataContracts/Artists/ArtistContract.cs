using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistContract {

		public ArtistContract() {}

		public ArtistContract(Artist artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Name = artist.TranslatedName[preference];
			Version = artist.Version;

		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

}
