﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models {

	public class SongList {

		public string Filter { get; set; }

	}

	public class SongDetails {

		public SongDetails(SongDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AdditionalNames = contract.AdditionalNames;
			Albums = contract.Albums;
			AlternateVersions = contract.AlternateVersions;
			ArtistString = contract.ArtistString;
			CanEdit = EntryPermissionManager.CanEdit(MvcApplication.LoginManager, contract.Song);
			CommentCount = contract.CommentCount;
			Deleted = contract.Deleted;
			Draft = contract.Song.Status == EntryStatus.Draft;
			Id = contract.Song.Id;
			IsFavorited = contract.IsFavorited;
			LatestComments = contract.LatestComments;
			Lyrics = contract.Lyrics;
			Name = contract.Song.Name;
			NicoId = contract.Song.NicoId;
			Notes = contract.Notes;
			OriginalVersion = contract.OriginalVersion;
			PVs = contract.PVs;
			SongType = contract.Song.SongType;
			Status = contract.Song.Status;
			Tags = contract.Tags;
			WebLinks = contract.WebLinks;

			Performers = contract.Artists.Select(a => a.Artist).Where(a => ArtistHelper.VocalistTypes.Contains(a.ArtistType)).ToArray();
			Producers = contract.Artists.Select(a => a.Artist).Where(a => a.ArtistType == Model.Domain.Artists.ArtistType.Producer).ToArray();
			OtherArtists = contract.Artists.Select(a => a.Artist).Where(a => !Performers.Contains(a) && !Producers.Contains(a)).ToArray();

			PrimaryPV = PVHelper.PrimaryPV(PVs);

			if (PrimaryPV == null && !string.IsNullOrEmpty(NicoId))
				PrimaryPV = new PVContract { PVId = NicoId, Service = PVService.NicoNicoDouga };

			var nicoPvId = PVHelper.GetNicoId(PVs, NicoId);

			if (!string.IsNullOrEmpty(nicoPvId))
				WebLinks = WebLinks.Concat(new[] { 
					new WebLinkContract(VideoServiceUrlFactory.NicoSound.CreateUrl(nicoPvId), "Check NicoSound for a download link"),
					new WebLinkContract(VideoServiceUrlFactory.NicoMimi.CreateUrl(nicoPvId), "Check NicoMimi for a download link")
				}).ToArray();

		}

		public string AdditionalNames { get; set; }

		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		[Display(Name = "Alternate versions")]
		public SongWithAdditionalNamesContract[] AlternateVersions { get; set; }

		public string ArtistString { get; set; }

		public bool CanEdit { get; set; }

		public int CommentCount { get; set; }

		public bool Deleted { get; set; }

		public bool Draft { get; set; }

		public int Id { get; set; }

		public bool IsFavorited { get; set; }

		public CommentContract[] LatestComments { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public string Notes { get; set; }

		public ArtistWithAdditionalNamesContract[] OtherArtists { get; set; }

		[Display(Name = "Original version")]
		public SongWithAdditionalNamesContract OriginalVersion { get; set; }

		public ArtistWithAdditionalNamesContract[] Performers { get; set; }

		public PVContract PrimaryPV { get; set; }

		public ArtistWithAdditionalNamesContract[] Producers { get; set; }

		[Display(Name = "PVs")]
		public PVContract[] PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		public EntryStatus Status { get; set; }

		public TagUsageContract[] Tags { get; set; }

		[Display(Name = "External links")]
		public WebLinkContract[] WebLinks { get; set; }

	}

	public class SongEdit {

		public SongEdit() {

			ArtistLinks = new List<ArtistForSongContract>();
			Lyrics = new List<LyricsForSongModel>();
			Names = new List<LocalizedStringEdit>();
			PVs = new List<PVContract>();
			WebLinks = new List<WebLinkDisplay>();

			AllPVTypes = EnumVal<PVType>.Values;
			AllVideoServices = EnumVal<PVService>.Values;

		}

		public SongEdit(SongForEditContract song)
			: this() {

			ParamIs.NotNull(() => song);

			ArtistLinks = song.Artists;
			DefaultLanguageSelection = song.TranslatedName.DefaultLanguage;
			Id = song.Song.Id;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongModel(l)).ToArray();
			Name = song.Song.Name;
			NameEnglish = song.TranslatedName.English;
			NameJapanese = song.TranslatedName.Japanese;
			NameRomaji = song.TranslatedName.Romaji;
			Names = song.Names.Select(l => new LocalizedStringEdit(l)).ToArray();
			Notes = song.Notes;
			OriginalVersion = song.OriginalVersion ?? new SongWithAdditionalNamesContract();
			PVs = song.PVs;
			SongType = song.Song.SongType;
			Status = song.Song.Status;
			WebLinks = song.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			CopyNonEditableFields(song);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public PVType[] AllPVTypes { get; set; }

		public PVService[] AllVideoServices { get; set; }

		public IList<ArtistForSongContract> ArtistLinks { get; set; }

		[Display(Name = "Original language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public bool Deleted { get; set; }

		public bool Draft { get; set; }

		public int Id { get; set; }

		[Display(Name = "Lyrics")]
		public IList<LyricsForSongModel> Lyrics { get; set; }

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

		[Display(Name = "Notes")]
		[StringLength(300)]
		public string Notes { get; set; }

		[Display(Name = "Original version")]
		public SongWithAdditionalNamesContract OriginalVersion { get; set; }

		public int OriginalVersionId { get; set; }

		[Display(Name = "PVs")]
		public IList<PVContract> PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		[Display(Name = "Entry status")]
		public EntryStatus Status { get; set; }

		public Model.Service.EntryValidators.ValidationResult ValidationResult { get; set; }

		[Display(Name = "Web links")]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(SongForEditContract song) {

			ParamIs.NotNull(() => song);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			Deleted = song.Deleted;
			Draft = song.Song.Status == EntryStatus.Draft;
			ValidationResult = song.ValidationResult;

		}

		public SongForEditContract ToContract() {

			return new SongForEditContract {
				Song = new SongContract {
					Status = this.Status,
					Id = this.Id,
					Name = this.Name,
					SongType = this.SongType
				},
				Artists = this.ArtistLinks.ToArray(),
				Lyrics = this.Lyrics.Select(l => l.ToContract()).ToArray(),
				Names = this.Names.Select(n => n.ToContract()).ToArray(),
				Notes = this.Notes ?? string.Empty,
				OriginalVersion = this.OriginalVersion ?? new SongWithAdditionalNamesContract { Id = OriginalVersionId },
				PVs = this.PVs.Select(p => p.NullToEmpty()).ToArray(),
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray(),
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),
			};

		}

	}

	/*public class PVForSongModel {

		public PVForSongModel() { }

		public PVForSongModel(PVForSongContract contract) {

		}

		public int Id { get; set; }

	}*/

	/*public class LyricsEditorModel {

		public LyricsEditorModel() {}

		public LyricsEditorModel(SongEdit model) {

			Id = model.Id;
			Lyrics = model.Lyrics;

		}

		public int Id { get; set; }

		public LyricsForSongModel[] Lyrics { get; set; }

	}*/

	public class LyricsForSongModel {
		
		public LyricsForSongModel() {}

		public LyricsForSongModel(LyricsForSongContract contract) {
			
			ParamIs.NotNull(() => contract);

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			Id = contract.Id;
			Language = contract.Language;
			Source = contract.Source;
			Value = contract.Value;

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public int Id { get; set; }

		[Required]
		[Display(Name = "Language")]
		public ContentLanguageSelection Language { get; set; }

		[Display(Name = "Source")]
		[StringLength(255)]
		public string Source { get; set; }

		[Required]
		[Display(Name = "Text")]
		public string Value { get; set; }

		public LyricsForSongContract ToContract() {

			return new LyricsForSongContract {
				Id = this.Id,
				Language = this.Language,
				Source = this.Source ?? string.Empty,
				Value = this.Value
			};

		}

	}

}