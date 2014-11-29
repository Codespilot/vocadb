using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Album {

	[PropertyModelBinder]
	public class AlbumEditViewModel {

		public AlbumEditViewModel() {
			
			Identifiers = new List<string>();
			Names = new List<LocalizedStringWithIdContract>();
			Pictures = new List<EntryPictureFileContract>();
			PVs = new List<PVContract>();
			Tracks = new List<SongInAlbumEditContract>();
			WebLinks = new List<WebLinkDisplay>();

			AllDiscTypes = Translate.DiscTypeNames.ValuesAndNames;

			DiscTypeDescriptions = ViewRes.Album.EditStrings.BaDiscTypeExplanation 
			                       + "<br /><br /><ul>" + string.Join("", 
				                       EnumVal<DiscType>.Values.Where(v => v != DiscType.Unknown).Select(v => string.Format("<li><strong>{0}</strong>: {1}</li>", 
					                       Translate.DiscTypeName(v), global::Resources.DiscTypeDescriptions.ResourceManager.GetString(v.ToString()))));


		}

		public AlbumEditViewModel(AlbumForEditContract album)
			: this() {

			ParamIs.NotNull(() => album);

			ArtistLinks = album.ArtistLinks;
			DefaultLanguageSelection = album.DefaultNameLanguage;
			Description = album.Description;
			DiscType = album.DiscType;
			Id = album.Id;
			Identifiers = album.Identifiers;
			Name = album.Name;
			Names = album.Names;
			Pictures = album.Pictures;
			PVs = album.PVs;
			Status = album.Status;
			Tracks = album.Songs;
			WebLinks = album.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			if (album.OriginalRelease != null) {
				CatNum = album.OriginalRelease.CatNum;
				ReleaseEvent = album.OriginalRelease.EventName;
				var d = album.OriginalRelease.ReleaseDate;
				if (d != null) {
					ReleaseDay = d.Day;
					ReleaseMonth = d.Month;
					ReleaseYear = d.Year;
				}
			}

			CopyNonEditableFields(album);

		}

		public Dictionary<DiscType, string> AllDiscTypes { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[FromJson]
		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[Display(Name = "Catalog number")]
		[StringLength(50)]
		public string CatNum { get; set; }

		[Display(Name = "Original language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public bool Deleted { get; set; }

		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Record type")]
		public DiscType DiscType { get; set; }

		public string DiscTypeDescriptions { get; set; }

		public bool Draft { get; set; }

		public bool HasCoverPicture { get; set; }

		public int Id { get; set; }

		[FromJson]
		public IList<string> Identifiers { get; set; }

		public string Name { get; set; }

		[Display(Name = "Names")]
		[FromJson]
		public IList<LocalizedStringWithIdContract> Names { get; set; }

		[FromJson]
		public IList<EntryPictureFileContract> Pictures { get; set; }

		[Display(Name = "PVs")]
		[FromJson]
		public IList<PVContract> PVs { get; set; }

		[Display(Name = "Day")]
		[Range(1, 31)]
		public int? ReleaseDay { get; set; }

		[Display(Name = "Month")]
		[Range(1, 12)]
		public int? ReleaseMonth { get; set; }

		[Display(Name = "Year")]
		public int? ReleaseYear { get; set; }

		[Display(Name = "Release event")]
		[StringLength(50)]
		public string ReleaseEvent { get; set; }

		[Display(Name = "Entry status")]
		public EntryStatus Status { get; set; }

		[Display(Name = "Tracks")]
		[FromJson]
		public IList<SongInAlbumEditContract> Tracks { get; set; }

		[StringLength(200)]
		public string UpdateNotes { get; set; }

		[Display(Name = "External links")]
		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(AlbumForEditContract album) {

			ParamIs.NotNull(() => album);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			Deleted = album.Deleted;
			Draft = album.Status == EntryStatus.Draft;
			HasCoverPicture = !string.IsNullOrEmpty(album.CoverPictureMime);
			Name = album.Name;

		}

		public AlbumForEditContract ToContract() {

			if (ArtistLinks == null)
				throw new InvalidFormException("Artists list was null");

			if (Identifiers == null)
				throw new InvalidFormException("Identifiers list was null");

			if (Names == null)
				throw new InvalidFormException("Names list was null");

			if (Tracks == null)
				throw new InvalidFormException("Tracks list was null");

			return new AlbumForEditContract {
				ArtistLinks = this.ArtistLinks,
				DefaultNameLanguage = this.DefaultLanguageSelection,
				Description = this.Description ?? string.Empty,
				DiscType = this.DiscType,
				Id = this.Id,
				Identifiers = this.Identifiers.ToArray(),
				Name = this.Name,
				Names = this.Names.ToArray(),
				OriginalRelease = new AlbumReleaseContract {
					CatNum = this.CatNum,
					EventName = this.ReleaseEvent,
					ReleaseDate = new OptionalDateTimeContract {
						Day = this.ReleaseDay,
						Month = this.ReleaseMonth,
						Year = this.ReleaseYear
					}
				},
				Pictures = this.Pictures.Select(p => p.NullToEmpty()).ToArray(),
				PVs = this.PVs.Select(p => p.NullToEmpty()).ToArray(),
				Songs = Tracks.ToArray(),
				Status = this.Status,				
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray()
			};

		}

		public object ToJson() {
			
			var model = new {
				artistLinks = ArtistLinks, 
				discType = DiscType.ToString(), 
				hasCover = HasCoverPicture, 
				identifiers = Identifiers,
				names = Names,
				pictures = Pictures,
				pvs = PVs,
				tracks = Tracks, 
				webLinks = WebLinks
			};

			return model;

		}

	}
}