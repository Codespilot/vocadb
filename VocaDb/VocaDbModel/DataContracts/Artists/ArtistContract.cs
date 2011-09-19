using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract]
	public class ArtistContract {

		public ArtistContract() {}

		public ArtistContract(Artist artist) {

			ParamIs.NotNull(() => artist);

			ArtistType = artist.ArtistType;
			Id = artist.Id;
			Name = artist.Name;

		}

		[DataMember]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
