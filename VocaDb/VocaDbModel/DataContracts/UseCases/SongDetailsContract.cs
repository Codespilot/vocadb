using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract]
	public class SongDetailsContract {

		public SongDetailsContract(Song song, IEnumerable<SongInRanking> songInPolls) {

			Song = new SongContract(song);

			Albums = song.Albums.Select(a => new AlbumContract(a.Album)).ToArray();
			AdditionalNames = string.Join(", ", song.LocalizedName.All.Where(n => n != song.Name));
			Artists = song.Artists.Select(a => new ArtistContract(a.Artist)).ToArray();
			WVRPlacements = songInPolls.Select(s => new SongWVRPlacementContract(s)).ToArray();

		}

		[DataMember]
		public AlbumContract[] Albums { get; private set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistContract[] Artists { get; private set; }

		[DataMember]
		public SongContract Song { get; private set; }

		[DataMember]
		public SongWVRPlacementContract[] WVRPlacements { get; private set; }

	}

}
