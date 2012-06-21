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
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models {

	public class ArtistIndex {

		public ArtistIndex() {}

		public ArtistIndex(PartialFindResult<ArtistWithAdditionalNamesContract> result, string filter,
			ArtistType artistType, bool? draftsOnly, ArtistSortRule sort, int? page) {

			Artists = new StaticPagedList<ArtistWithAdditionalNamesContract>(result.Items, page ?? 1, 30, result.TotalCount);
			DraftsOnly = draftsOnly ?? false;
			Filter = filter;
			ArtistType = artistType;
			Sort = sort;

			FilterableArtistTypes = EnumVal<ArtistType>.Values.ToDictionary(a => a, Translate.ArtistTypeName);

		}

		public IPagedList<ArtistWithAdditionalNamesContract> Artists { get; set; }

		public ArtistType ArtistType { get; set; }

		[Display(ResourceType = typeof(ViewRes.EntryIndexStrings), Name = "OnlyDrafts")]
		public bool DraftsOnly { get; set; }

		public string Filter { get; set; }

		public Dictionary<ArtistType, string> FilterableArtistTypes { get; set; }

		public ArtistSortRule Sort { get; set; }

	}

	public class ArtistEdit {

		public ArtistEdit() {

			AlbumLinks = new List<AlbumForArtistEditContract>();
			Groups = new List<GroupForArtistContract>();
			Names = new List<LocalizedStringEdit>();
			WebLinks = new List<WebLinkDisplay>();

			AllArtistTypes = EnumVal<ArtistType>.Values.ToDictionary(a => a, Translate.ArtistTypeName);

		}

		public ArtistEdit(ArtistForEditContract artist)
			: this() {

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
			Status = artist.Status;
			TooManyAlbums = artist.TooManyAlbums;
			WebLinks = artist.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			CopyNonEditableFields(artist);

		}

		public IList<AlbumForArtistEditContract> AlbumLinks { get; set; }

		public Dictionary<ArtistType, string> AllArtistTypes { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[Display(Name = "Artist type")]
		public ArtistType ArtistType { get; set; }

		[Display(Name = "Original language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public bool Deleted { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "This entry is a draft")]
		public bool Draft { get; set; }

		[Display(Name = "Groups and circles")]
		public IList<GroupForArtistContract> Groups { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		[Display(Name = "Names")]
		public IList<LocalizedStringEdit> Names { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Name in Japanese")]
		[StringLength(255)]
		public string NameJapanese { get; set; }

		[Display(Name = "Name in Romaji")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(Name = "Entry status")]
		public EntryStatus Status { get; set; }

		public bool TooManyAlbums { get; set; }

		public string UpdateNotes { get; set; }

		public Model.Service.EntryValidators.ValidationResult ValidationResult { get; set; }

		[Display(Name = "Web links")]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(ArtistForEditContract artist) {

			ParamIs.NotNull(() => artist);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			Deleted = artist.Deleted;
			Draft = artist.Status == EntryStatus.Draft;
			ValidationResult = artist.ValidationResult;

		}

		public ArtistForEditContract ToContract() {

			return new ArtistForEditContract {
				
				Id = this.Id,
				AlbumLinks = this.AlbumLinks.ToArray(),
				ArtistType = this.ArtistType,
				Description =  this.Description ?? string.Empty,
				Groups = this.Groups.ToArray(),
				Name = this.Name,
				Names = this.Names.Select(l => l.ToContract()).ToArray(),
				Status = this.Status,
				TooManyAlbums = this.TooManyAlbums,
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray()

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