using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedPVContract {

		public ArchivedPVContract() {}

		public ArchivedPVContract(PVForSong pvForSong) {
			
			ParamIs.NotNull(() => pvForSong);

			PVId = pvForSong.PVId;
			Service = pvForSong.Service;
			PVType = pvForSong.PVType;

		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		public PVService Service { get; set; }

		[DataMember]
		public PVType PVType { get; set; }

	}

}
