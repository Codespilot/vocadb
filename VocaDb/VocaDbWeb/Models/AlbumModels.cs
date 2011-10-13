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
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Web.Models {

	public class AlbumEdit {

		public AlbumEdit() {}

		public AlbumEdit(AlbumForEditContract album) {

			ParamIs.NotNull(() => album);

			AllLabels = album.AllLabels;
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
			Tracks = album.Songs;
			WebLinks = album.WebLinks;

			if (album.OriginalRelease != null) {
				CatNum = album.OriginalRelease.CatNum;
				Label = album.OriginalRelease.Label;
				ReleaseDate = album.OriginalRelease.ReleaseDate;				
			}

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);
			AllDiscTypes = EnumVal<DiscType>.Values;

		}

		public DiscType[] AllDiscTypes { get; set; }

		public ArtistContract[] AllLabels { get; set; }

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[Display(Name = "Catalog number")]
		[StringLength(50)]
		public string CatNum { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Record type")]
		public DiscType DiscType { get; set; }

		public int Id { get; set; }

		[Display(Name = "Original release label")]
		public ArtistContract Label { get; set; }

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

		[Display(Name = "Release date")]
		public DateTime? ReleaseDate { get; set; }

		[Display(Name = "Release event")]
		[StringLength(50)]
		public string ReleaseEvent { get; set; }

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
				OriginalRelease = new AlbumReleaseContract {
					CatNum = this.CatNum,
					Label = this.Label,
					ReleaseDate = this.ReleaseDate,
					EventName = this.ReleaseEvent
				},
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),				
			};

		}

	}

	public class AlbumDetails {

		public AlbumDetails() { }

		public AlbumDetails(AlbumDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AdditionalNames = contract.AdditionalNames;
			Description = contract.Description;
			DiscType = contract.DiscType;
			Id = contract.Id;
			Name = contract.Name;
			Songs = contract.Songs;
			UserHasAlbum = contract.UserHasAlbum;
			WebLinks = contract.WebLinks;

			OtherArtists = contract.ArtistLinks.Where(a => a.Artist.ArtistType != ArtistType.Performer).Select(a => a.Artist).ToArray();
			Performers = contract.ArtistLinks.Where(a => a.Artist.ArtistType == ArtistType.Performer).Select(a => a.Artist).ToArray();

		}

		public string AdditionalNames { get; set; }

		public string Description { get; set; }

		public DiscType DiscType { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ArtistContract[] OtherArtists { get; set; }

		public ArtistContract[] Performers { get; set; }

		public SongInAlbumContract[] Songs { get; set; }

		public bool UserHasAlbum { get; set; }

		public WebLinkContract[] WebLinks { get; set; }


	}

}