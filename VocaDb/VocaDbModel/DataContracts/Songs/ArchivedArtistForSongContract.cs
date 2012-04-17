using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedArtistForSongContract : ObjectRefContract {

		public ArchivedArtistForSongContract() { }

		public ArchivedArtistForSongContract(ArtistForSong entry)
			: base(entry.Artist) {

			IsSupport = entry.IsSupport;
			Roles = entry.Roles;

		}

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }

	}

}
