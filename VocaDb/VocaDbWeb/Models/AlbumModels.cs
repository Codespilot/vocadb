using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models {

	public class AlbumEdit {

		public AlbumEdit(AlbumForEditContract album) {

			Description = album.Description;
			Id = album.Id;
			Name = album.Name;
			NameEnglish = album.TranslatedName.English;
			NameJapanese = album.TranslatedName.Japanese;
			NameRomaji = album.TranslatedName.Romaji;
			Names = album.Names;
			WebLinks = album.WebLinks;

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; protected set; }

		[Display(Name = "Names")]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[Required]
		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Required]
		[Display(Name = "Name in Japanese")]
		[StringLength(255)]
		public string NameJapanese { get; set; }

		[Required]
		[Display(Name = "Name in Romaji")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(Name = "Web links")]
		public WebLinkContract[] WebLinks { get; set; }

	}

}