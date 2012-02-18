using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared {

	public class WebLinkDisplay {

		public WebLinkDisplay() {
			Description = string.Empty;
			Url = string.Empty;
		}

		public WebLinkDisplay(WebLinkContract contract) {
			
			ParamIs.NotNull(() => contract);

			Description = contract.Description;
			Id = contract.Id;
			Url = contract.Url;

		}

		[StringLength(512)]
		public string Description { get; set; }

		public int Id { get; set; }

		[StringLength(512)]
		[DataType(DataType.Url)]
		public string Url { get; set; }

		public WebLinkContract ToContract() {

			return new WebLinkContract { Id = this.Id, Description = this.Description ?? string.Empty, Url = this.Url };

		}

	}

}