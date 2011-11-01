﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models.Artist {

	public class Create {

		public Create() {
			ArtistType = ArtistType.Producer;
			Description = string.Empty;
		}

		[Display(Name = "Please write a short description about this artist (optional, but recommended)")]
		[StringLength(1000)]
		public string Description { get; set; }

		[Display(Name = "Disc type")]
		public ArtistType ArtistType { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Original name")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(Name = "Romanized name")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		public CreateArtistContract ToContract() {

			return new CreateArtistContract {
				ArtistType = this.ArtistType,
				Description = this.Description,
				Names = LocalizedStringHelper.SkipNull(NameOriginal, NameRomaji, NameEnglish).ToArray()
			};

		}

	}
}