using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedPVContract {

		public ArchivedPVContract() {}

		public ArchivedPVContract(PV pv) {

			ParamIs.NotNull(() => pv);

			Name = pv.Name;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;

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
