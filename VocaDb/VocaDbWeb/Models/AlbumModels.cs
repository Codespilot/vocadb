using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models {

	public class AlbumEdit {

		public AlbumEdit() {}

		public AlbumEdit(AlbumForEditContract album) {

			ParamIs.NotNull(() => album);

			ArtistLinks = album.ArtistLinks;
			DefaultLanguageSelection = album.TranslatedName.DefaultLanguage;
			Description = album.Description;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.Name;
			NameEnglish = album.TranslatedName.English;
			NameJapanese = album.TranslatedName.Japanese;
			NameRomaji = album.TranslatedName.Romaji;
			Names = album.Names;
			ProductCode = album.ProductCode;
			ReleaseDate = album.ReleaseDate;
			Tracks = album.Songs;
			WebLinks = album.WebLinks;

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);
			AllDiscTypes = EnumVal<DiscType>.Values;

		}

		public DiscType[] AllDiscTypes { get; set; }

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Record type")]
		public DiscType DiscType { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

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

		[Display(Name = "Catalog number / Product code")]
		[StringLength(50)]
		public string ProductCode { get; set; }

		[Display(Name = "Release date")]
		public DateTime? ReleaseDate { get; set; }

		[Display(Name = "Tracks")]
		public SongInAlbumContract[] Tracks { get; set; }

		[Display(Name = "Web links")]
		public WebLinkContract[] WebLinks { get; set; }

		public AlbumForEditContract ToContract() {

			return new AlbumForEditContract {
				Description = this.Description ?? string.Empty,
				DiscType = this.DiscType,
				Id = this.Id,
				Name = this.Name,
				ProductCode = this.ProductCode,
				ReleaseDate = this.ReleaseDate,
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),				
			};

		}

	}

}