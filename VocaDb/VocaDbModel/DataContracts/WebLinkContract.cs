using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts {

	public class WebLinkContract {

		public WebLinkContract(WebLink link) {
			
			ParamIs.NotNull(() => link);

			Description = link.Description;
			Id = link.Id;
			Url = link.Url;

		}

		public string Description { get; set; }

		public int Id { get; set; }

		public string Url { get; set; }

	}

}
