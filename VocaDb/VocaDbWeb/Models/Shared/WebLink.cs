using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared {

	public class WebLink {

		public WebLink() {
			Description = string.Empty;
			Url = string.Empty;
		}

		public WebLink(WebLinkContract contract) {
			
			ParamIs.NotNull(() => contract);

			Description = contract.Description;
			Id = contract.Id;
			Url = contract.Url;

		}

		public string Description { get; set; }

		public int Id { get; set; }

		[Required]
		public string Url { get; set; }

		public WebLinkContract ToContract() {

			return new WebLinkContract { Id = this.Id, Description = this.Description ?? string.Empty, Url = this.Url };

		}

	}

}