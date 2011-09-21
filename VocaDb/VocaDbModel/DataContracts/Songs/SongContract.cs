using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class SongContract {

		public SongContract() {}

		public SongContract(Song song) {

			ParamIs.NotNull(() => song);

			ArtistString = song.ArtistString;
			CreateDate = song.CreateDate;
			Id = song.Id;
			Name = song.Name;
			NicoId = song.NicoId;

		}

		[DataMember]
		public string ArtistString { get; set; }

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
