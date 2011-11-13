using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumDiffContract {

		public AlbumDiffContract()
			: this(true) { }

		public AlbumDiffContract(bool isFullDiff) {
			Cover = Description = Names = WebLinks = IsFullDiff = isFullDiff;
		}

		[DataMember]
		public bool Cover { get; set; }

		[DataMember]
		public bool Description { get; set; }

		[DataMember]
		public bool IsFullDiff { get; set; }

		[DataMember]
		public bool Names { get; set; }

		[DataMember]
		public bool WebLinks { get; set; }

	}
}
