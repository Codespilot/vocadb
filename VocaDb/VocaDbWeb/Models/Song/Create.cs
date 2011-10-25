using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models.Song {

	public class Create {

		[Display(Name = "Artists")]
		public ArtistContract[] Artists { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Original name *")]
		[Required]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(Name = "URL to the original PV (Youtube or NND)")]
		[StringLength(255)]
		public string PVUrl { get; set; }

		[Display(Name = "Romanized name")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		public CreateSongContract ToContract() {

			var nameContract = new TranslatedStringContract(NameOriginal, NameOriginal, NameOriginal, ContentLanguageSelection.Japanese);

			if (!string.IsNullOrEmpty(NameRomaji))
				nameContract.Romaji = NameRomaji;

			if (!string.IsNullOrEmpty(NameEnglish))
				nameContract.English = NameEnglish;

			return new CreateSongContract {
				Artists = this.Artists,
				Name = nameContract,
				PVUrl = this.PVUrl,
			};

		}

	}

}