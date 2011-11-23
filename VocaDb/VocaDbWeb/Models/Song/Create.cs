using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models.Song {

	public class Create {

		public Create() {
			Artists = new List<ArtistContract>();
			SongType = SongType.Original;
		}

		[Display(Name = "Artists")]
		public IList<ArtistContract> Artists { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Non-English name")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(Name = "URL to the original PV (NicoNicoDouga or Youtube)")]
		[StringLength(255)]
		public string PVUrl { get; set; }

		[Display(Name = "Romanized name")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(Name = "URL to the reprint of the PV (NicoNicoDouga or Youtube)")]
		[StringLength(255)]
		public string ReprintPVUrl { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		public CreateSongContract ToContract() {

			return new CreateSongContract {
				Artists = this.Artists.ToArray(),
				Names = LocalizedStringHelper.SkipNullAndEmpty(NameOriginal, NameRomaji, NameEnglish).ToArray(),
				PVUrl = this.PVUrl,
				ReprintPVUrl = this.ReprintPVUrl,
				SongType = this.SongType
			};

		}

	}

}