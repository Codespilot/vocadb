using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract]
	public class ArtistContract {

		public ArtistContract() {}

		public ArtistContract(Artist artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Name = artist.TranslatedName[preference];

		}

		[DataMember]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
