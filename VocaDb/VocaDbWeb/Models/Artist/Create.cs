using System.Linq;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Artist {

	public class Create {

		public Create() {
			ArtistType = ArtistType.Producer;
			Description = string.Empty;
		}

		[Display(Name = "Please write a short description about this artist (optional, but recommended)")]
		[StringLength(1000)]
		public string Description { get; set; }

		[Display(Name = "Artist type")]
		public ArtistType ArtistType { get; set; }

		[Display(Name = "Keep as draft")]
		public bool Draft { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Non-English name")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(Name = "Romanized name")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(Name = "Description")]
		[StringLength(512)]
		public string WebLinkDescription { get; set; }

		[Display(Name = "URL")]
		[StringLength(512)]
		public string WebLinkUrl { get; set; }

		public CreateArtistContract ToContract() {

			return new CreateArtistContract {
				ArtistType = this.ArtistType,
				Description = this.Description ?? string.Empty,
				Draft = this.Draft,
				Names = LocalizedStringHelper.SkipNullAndEmpty(NameOriginal, NameRomaji, NameEnglish).ToArray(),
				WebLink = (!string.IsNullOrWhiteSpace(WebLinkUrl) ? new WebLinkContract { Description = WebLinkDescription, Url = WebLinkUrl } : null)
			};

		}

	}
}