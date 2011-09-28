using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models {

	public class ArtistEdit {

		class EmptyArtist : ArtistContract {

			public const int EmptyArtistId = 0;

			public EmptyArtist() {

				Id = EmptyArtistId;
				Name = "(nothing)";

			}

		}

		public ArtistEdit() {}

		public ArtistEdit(ArtistForEditContract artist) {

			Albums = artist.Albums;
			ArtistType = artist.ArtistType;
			CircleId = (artist.Circle != null ? artist.Circle.Id : EmptyArtist.EmptyArtistId);
			DefaultLanguageSelection = artist.TranslatedName.DefaultLanguage;
			Description = artist.Description;
			Id = artist.Id;
			Name = artist.Name;
			NameEnglish = artist.TranslatedName.English;
			NameJapanese = artist.TranslatedName.Japanese;
			NameRomaji = artist.TranslatedName.Romaji;
			Names = artist.Names;
			WebLinks = artist.WebLinks;

			AllArtistTypes = EnumVal<ArtistType>.Values;
			AllCircles = new[] { new EmptyArtist() }.Concat(artist.AllCircles)
				.ToDictionary(a => a.Id, a => a.Name);
			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);

		}

		public AlbumContract[] Albums { get; set; }

		public ArtistType[] AllArtistTypes { get; set; }

		public Dictionary<int, string> AllCircles { get; set; }

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		[Display(Name = "Artist type")]
		public ArtistType ArtistType { get; set; }

		[Display(Name = "Circle")]
		public int CircleId { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

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

		[Display(Name = "Web links")]
		public WebLinkContract[] WebLinks { get; set; }

		public ArtistDetailsContract ToContract() {

			return new ArtistDetailsContract {
				
				Id = this.Id,
				ArtistType = this.ArtistType,
				Circle = (CircleId != EmptyArtist.EmptyArtistId ? new ArtistContract { Id = CircleId} : null),
				Description =  this.Description ?? string.Empty,
				Name = this.Name,
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),				

			};

		}

	}

}