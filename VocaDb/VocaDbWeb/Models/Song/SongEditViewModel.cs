using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Song {

	[PropertyModelBinder]
	public class SongEditViewModel {

		public SongEditViewModel() {

			ArtistLinks = new List<ArtistForSongContract>();
			Lyrics = new List<LyricsForSongModel>();
			Names = new List<LocalizedStringWithIdContract>();
			OriginalVersion = new SongContract();
			PVs = new List<PVContract>();
			WebLinks = new List<WebLinkDisplay>();

			AllPVTypes = EnumVal<PVType>.Values;
			AllVideoServices = EnumVal<PVService>.Values;

		}

		public SongEditViewModel(SongForEditContract song)
			: this() {

			ParamIs.NotNull(() => song);

			ArtistLinks = song.Artists;
			DefaultLanguageSelection = song.TranslatedName.DefaultLanguage;
			Id = song.Song.Id;
			Length = song.Song.LengthSeconds;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongModel(l)).ToArray();
			NameEnglish = song.TranslatedName.English;
			NameJapanese = song.TranslatedName.Japanese;
			NameRomaji = song.TranslatedName.Romaji;
			Names = song.Names;
			Notes = song.Notes;
			OriginalVersion = song.OriginalVersion ?? new SongContract();
			PVs = song.PVs;
			SongType = song.Song.SongType;
			Status = song.Song.Status;
			WebLinks = song.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			CopyNonEditableFields(song);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public PVType[] AllPVTypes { get; set; }

		public PVService[] AllVideoServices { get; set; }

		[FromJson]
		public IList<ArtistForSongContract> ArtistLinks { get; set; }

		[Display(Name = "Original language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public bool Deleted { get; set; }

		public bool Draft { get; set; }

		public int Id { get; set; }

		public int Length { get; set; }

		[Display(Name = "Lyrics")]
		public IList<LyricsForSongModel> Lyrics { get; set; }

		public string Name { get; set; }

		[Display(Name = "Names (at least one)")]
		[FromJson]
		public IList<LocalizedStringWithIdContract> Names { get; set; }

		[Display(Name = "Name in English")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(Name = "Name in Japanese")]
		[StringLength(255)]
		public string NameJapanese { get; set; }

		[Display(Name = "Name in Romaji")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		[Display(Name = "Notes")]
		[StringLength(800)]
		public string Notes { get; set; }

		[Display(Name = "Original version")]
		public SongContract OriginalVersion { get; set; }

		public int OriginalVersionId { get; set; }

		[Display(Name = "PVs")]
		[FromJson]
		public IList<PVContract> PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		[Display(Name = "Entry status")]
		public EntryStatus Status { get; set; }

		[StringLength(200)]
		public string UpdateNotes { get; set; }

		[Display(Name = "Web links")]
		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(SongForEditContract song) {

			ParamIs.NotNull(() => song);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			Deleted = song.Deleted;
			Draft = song.Song.Status == EntryStatus.Draft;
			Name = song.Song.Name;

		}

		public SongForEditContract ToContract() {

			if (ArtistLinks == null)
				throw new InvalidFormException("ArtistLinks list was null"); // Shouldn't be null

			if (Lyrics == null)
				throw new InvalidFormException("Lyrics list was null"); // Shouldn't be null

			if (Names == null)
				throw new InvalidFormException("Names list was null"); // Shouldn't be null

			if (PVs == null)
				throw new InvalidFormException("PVs list was null"); // Shouldn't be null

			if (WebLinks == null)
				throw new InvalidFormException("WebLinks list was null"); // Shouldn't be null

			return new SongForEditContract {
				Song = new SongContract {
					Status = this.Status,
					Id = this.Id,
					LengthSeconds = this.Length,
					Name = this.Name,
					SongType = this.SongType
				},
				Artists = this.ArtistLinks.ToArray(),
				Lyrics = this.Lyrics.Select(l => l.ToContract()).ToArray(),
				Names = this.Names.ToArray(),
				Notes = this.Notes ?? string.Empty,
				OriginalVersion = this.OriginalVersion != null && this.OriginalVersion.Id != 0 ? this.OriginalVersion : new SongContract { Id = OriginalVersionId },
				PVs = this.PVs.Select(p => p.NullToEmpty()).ToArray(),
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray(),
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),
			};

		}

		public object ToJsonModel() {
			
			var model = new {
				ArtistLinks, 
				Length, 
				Names, 
				pvs = PVs,
				WebLinks
			};

			return model;

		}

	}
}