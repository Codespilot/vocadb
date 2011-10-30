using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Album {

	public class Create {

		public Create() {
			Artists = new List<ArtistContract>();
		}

		[Display(Name = "Artists")]
		public IList<ArtistContract> Artists { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Original name")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(Name = "Romanized name")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		public CreateAlbumContract ToContract() {

			return new CreateAlbumContract {
				/*Artists = this.Artists.ToArray(),
				NameEnglish = this.NameEnglish,
				NameRomaji = this.NameRomaji,
				NameOriginal = this.NameOriginal,
				PVUrl = this.PVUrl,
				ReprintPVUrl = this.ReprintPVUrl*/
			};

		}

	}

}