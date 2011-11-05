using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVContract {

		public PVContract(PV pv) {

			ParamIs.NotNull(() => pv);

			Id = pv.Id;
			Name = pv.Name;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;
			Url = pv.Url;

			DisplayName = Service + " (" + PVType + ")";

		}

		public PVContract() { }

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
