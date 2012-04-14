using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

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

		}

		public PVContract(PVForSong pv)
			: this((PV)pv) {

			Author = pv.Author;

		}

		public PVContract(VideoUrlParseResult parseResult, PVType type) {

			ParamIs.NotNull(() => parseResult);

			Name = parseResult.Title;
			PVId = parseResult.Id;
			Service = parseResult.Service;
			PVType = type;

			Url = PV.GetUrl(Service, PVId);
			
		}

		public PVContract() { }

		[DataMember]
		public string Author { get; set; }

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

		public PVContract NullToEmpty() {
			Name = Name ?? string.Empty;
			return this;
		}

	}

}
