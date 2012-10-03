using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.PVs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PVContract {

		public PVContract() { }

		public PVContract(PV pv) {

			ParamIs.NotNull(() => pv);

			Author = pv.Author;
			Id = pv.Id;
			Name = pv.Name;
			PVId = pv.PVId;
			Service = pv.Service;
			PVType = pv.PVType;
			Url = pv.Url;

		}

		public PVContract(ArchivedPVContract contract) {

			ParamIs.NotNull(() => contract);

			Author = contract.Author;
			Name = contract.Name;
			PVId = contract.PVId;
			Service = contract.Service;
			PVType = contract.PVType;

		}

		public PVContract(PVForSong pv)
			: this((PV)pv) {

			ThumbUrl = pv.ThumbUrl;

		}

		public PVContract(VideoUrlParseResult parseResult, PVType type) {

			ParamIs.NotNull(() => parseResult);

			Author = parseResult.Author;
			Name = parseResult.Title;
			PVId = parseResult.Id;
			Service = parseResult.Service;
			ThumbUrl = parseResult.ThumbUrl;
			PVType = type;

			Url = PV.GetUrl(Service, PVId);
			
		}

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
		public string ThumbUrl { get; set; }

		[DataMember]
		public string Url { get; set; }

		public PVContract NullToEmpty() {
			Author = Author ?? string.Empty;
			Name = Name ?? string.Empty;
			ThumbUrl = ThumbUrl ?? string.Empty;
			return this;
		}

	}

}
