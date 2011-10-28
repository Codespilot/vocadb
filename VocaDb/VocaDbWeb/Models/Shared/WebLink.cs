using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared {

	public class WebLink {

		public WebLink() {}

		public WebLink(WebLinkContract contract) {
			
			ParamIs.NotNull(() => contract);

			Description = contract.Description;
			Url = contract.Url;

		}

		public string Description { get; set; }

		[Required]
		public string Url { get; set; }

	}

}