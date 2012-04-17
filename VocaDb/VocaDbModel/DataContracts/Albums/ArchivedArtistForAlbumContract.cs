using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistForAlbumContract : ObjectRefContract {

		public ArchivedArtistForAlbumContract() {}

		public ArchivedArtistForAlbumContract(ArtistForAlbum entry) : base(entry.Artist) {

			Roles = entry.Roles;
			IsSupport = entry.IsSupport;

		}

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }

	}

}
