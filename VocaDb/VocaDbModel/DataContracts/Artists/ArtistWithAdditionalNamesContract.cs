using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract]
	public class ArtistWithAdditionalNamesContract : ArtistContract {

		public ArtistWithAdditionalNamesContract(Artist artist)
			: base(artist) {

			AdditionalNames = string.Join(", ", artist.AllNames.Where(n => n != artist.Name));

		}

		[DataMember]
		public string AdditionalNames { get; private set; }

	}

}
