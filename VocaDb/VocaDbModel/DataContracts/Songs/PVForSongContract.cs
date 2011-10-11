using VocaDb.Model.Domain.Songs;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace=Schemas.VocaDb)]
	public class PVForSongContract {

		public PVForSongContract(PVForSong pvForSong) {

			ParamIs.NotNull(() => pvForSong);

			Id = pvForSong.Id;
			Name = pvForSong.Name;
			PVId = pvForSong.PVId;
			Service = pvForSong.Service;
			PVType = pvForSong.PVType;
			Url = pvForSong.Url;

			DisplayName = Service + " (" + PVId + ")";

		}

		public PVForSongContract() { }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		public PVService Service { get; set; }

		[DataMember]
		public PVType PVType { get; set; }

		[DataMember]
		public string Url { get; set; }

	}
}
