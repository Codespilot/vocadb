using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistContract : IEntryWithStatus {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		EntryType IEntryBase.EntryType {
			get { return EntryType.Artist; }
		}

		public ArtistContract() {}

		public ArtistContract(Artist artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(preference);
			ArtistType = artist.ArtistType;
			Deleted = artist.Deleted;
			Id = artist.Id;
			Name = artist.TranslatedName[preference];
			Status = artist.Status;
			Version = artist.Version;

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

}
