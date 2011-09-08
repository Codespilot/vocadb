using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class WebLinkContract {

		public WebLinkContract(WebLink link) {
			
			ParamIs.NotNull(() => link);

			Description = link.Description;
			Url = link.Url;

		}

		public string Description { get; set; }

		public string Url { get; set; }

	}

}
