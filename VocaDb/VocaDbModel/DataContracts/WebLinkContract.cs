using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class WebLinkContract {

		public WebLinkContract() {}

		public WebLinkContract(string url, string description) {

			Url = url;
			Description = description;

			DescriptionOrUrl = !string.IsNullOrEmpty(description) ? description : url;

		}

		public WebLinkContract(WebLink link) {
			
			ParamIs.NotNull(() => link);

			Description = link.Description;
			DescriptionOrUrl = link.DescriptionOrUrl;
			Id = link.Id;
			Url = link.Url;

		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DescriptionOrUrl { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Url { get; set; }

	}

}
