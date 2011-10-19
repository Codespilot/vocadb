using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using PagedList;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models {

	public class ArtistIndex {

		public ArtistIndex() {}

		public ArtistIndex(PartialFindResult<ArtistWithAdditionalNamesContract> result, string filter, ArtistType artistType, int? page) {

			Artists = new StaticPagedList<ArtistWithAdditionalNamesContract>(result.Items.OrderBy(a => a.Name), page ?? 1, 30, result.TotalCount);
			Filter = filter;
			ArtistType = artistType;

			FilterableArtistTypes = EnumVal<ArtistType>.Values.ToDictionary(a => a, Translate.ArtistTypeName);

		}

		public IPagedList<ArtistWithAdditionalNamesContract> Artists { get; set; }

		public ArtistType ArtistType { get; set; }

		public string Filter { get; set; }

		public Dictionary<ArtistType, string> FilterableArtistTypes { get; set; }

	}

	public class ArtistEdit {

		public ArtistEdit() {}

		public ArtistEdit(ArtistForEditContract artist) {

			AlbumLinks = artist.AlbumLinks;
			ArtistType = artist.ArtistType;
			DefaultLanguageSelection = artist.TranslatedName.DefaultLanguage;
			Description = artist.Description;
			Groups = artist.Groups;
			Id = artist.Id;
			Name = artist.Name;
			NameEnglish = artist.TranslatedName.English;
			NameJapanese = artist.TranslatedName.Japanese;
			NameRomaji = artist.TranslatedName.Romaji;
			Names = artist.Names.Select(n => new LocalizedStringEdit(n)).ToArray();
			WebLinks = artist.WebLinks;

			AllArtistTypes = EnumVal<ArtistType>.Values.ToDictionary(a => a, Translate.ArtistTypeName);
			AllCircles = artist.AllCircles.ToDictionary(a => a.Id, a => a.Name);

		}

		public ArtistForAlbumContract[] AlbumLinks { get; set; }

		public Dictionary<ArtistType, string> AllArtistTypes { get; set; }

		public Dictionary<int, string> AllCircles { get; set; }

		[Display(Name = "Artist type")]
		public ArtistType ArtistType { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Groups and circles")]
		public GroupForArtistContract[] Groups { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		[Display(Name = "Names")]
		public LocalizedStringEdit[] Names { get; set; }

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
				Description =  this.Description ?? string.Empty,
				Groups = this.Groups ?? new GroupForArtistContract[] {},
				Name = this.Name,
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),				

			};

		}

	}

	public class EmptyArtist : ArtistContract {

		public const int EmptyArtistId = 0;

		public EmptyArtist() {

			Id = EmptyArtistId;
			Name = "(nothing)";

		}

	}

}