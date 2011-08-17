using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Ranking;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract]
	public class SongDetailsContract {

		public SongDetailsContract(Song song, IEnumerable<SongInRanking> songInPolls) {
			Song = new SongContract(song);
			WVRPlacements = songInPolls.Select(s => new SongWVRPlacementContract(s)).ToArray();
		}

		[DataMember]
		public SongContract Song { get; private set; }

		[DataMember]
		public SongWVRPlacementContract[] WVRPlacements { get; private set; }

	}

}
