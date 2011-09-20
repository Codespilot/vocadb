using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class ArtistForSongContract {

		public ArtistForSongContract(ArtistForSong artistForSong) {
			
			ParamIs.NotNull(() => artistForSong);

			Artist = new ArtistContract(artistForSong.Artist);
			Id = artistForSong.Id;

		}

		[DataMember]
		public ArtistContract Artist { get; set; }

		[DataMember]
		public int Id { get; set; }

	}

}
