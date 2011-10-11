using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedWebLinkContract {

		public ArchivedWebLinkContract() {}

		public ArchivedWebLinkContract(WebLink webLink) {
			
			ParamIs.NotNull(() => webLink);

			Description = webLink.Description;
			Url = webLink.Url;

		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Url { get; set; }

	}
}
