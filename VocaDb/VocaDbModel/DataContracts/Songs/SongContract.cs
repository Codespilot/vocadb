using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;
using VocaVoter.Model.Domain.Songs;

namespace VocaVoter.Model.DataContracts.Songs {

	[DataContract]
	public class SongContract {

		public SongContract() {}

		public SongContract(Song song) {
			CreateDate = song.CreateDate;
			Id = song.Id;
			Name = song.Name;
			NicoId = song.NicoId;
		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

	}

}
