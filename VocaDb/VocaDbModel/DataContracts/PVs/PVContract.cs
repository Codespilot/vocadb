using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVContract {

		public PVContract(PV pv) {

			ParamIs.NotNull(() => pv);

			Id = pv.Id;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;
			Url = pv.Url;

			DisplayName = Service + " (" + PVType + ")";

		}

		public PVContract(PVForSong pv)
			: this((PV)pv) {

			Author = pv.Author;
			Name = pv.Name;

		}

		public PVContract() { }

		[DataMember]
		public string Author { get; set; }

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
