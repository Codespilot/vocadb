using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.Song {

	public class Create {

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

	}

}