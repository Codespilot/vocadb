using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class GroupForArtistContract {

		public GroupForArtistContract() {}

		public GroupForArtistContract(GroupForArtist groupForArtist, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => groupForArtist);

			Group = new ArtistWithAdditionalNamesContract(groupForArtist.Group, languagePreference);
			Id = groupForArtist.Id;
			Member = new ArtistWithAdditionalNamesContract(groupForArtist.Member, languagePreference);

		}

		[DataMember]
		public ArtistWithAdditionalNamesContract Group { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public ArtistWithAdditionalNamesContract Member { get; set; }

	}

}
