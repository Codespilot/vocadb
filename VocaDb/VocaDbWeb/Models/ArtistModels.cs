using System.ComponentModel.DataAnnotations;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models {

	public class ArtistCreate {

		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; }

	}

	public class ArtistEdit {

		public ArtistEdit() {}

		public ArtistEdit(ArtistDetailsContract artist) {

			DefaultLanguageSelection = artist.LocalizedName.DefaultLanguage;
			Id = artist.Id;
			Name = artist.Name;
			NameEnglish = artist.LocalizedName.English;
			NameJapanese = artist.LocalizedName.Japanese;
			NameRomaji = artist.LocalizedName.Romaji;

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		[Required]
		[Display(Name = "Name in English")]
		public string NameEnglish { get; set; }

		[Required]
		[Display(Name = "Name in Japanese")]
		public string NameJapanese { get; set; }

		[Required]
		[Display(Name = "Name in Romaji")]
		public string NameRomaji { get; set; }

		public ArtistDetailsContract ToContract() {

			return new ArtistDetailsContract {
				
				Id = this.Id,
				LocalizedName = new LocalizedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection)

			};

		}

	}

}