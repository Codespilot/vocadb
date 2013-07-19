using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Web.Models {

	public class SongList {

		public string Filter { get; set; }

	}

	public class SongDetails {

		public SongDetails(SongDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AdditionalNames = contract.AdditionalNames;
			Albums = contract.Albums;
			AlternateVersions = contract.AlternateVersions.Where(a => a.SongType != SongType.Original).ToArray();
			ArtistString = contract.ArtistString;
			CanEdit = EntryPermissionManager.CanEdit(MvcApplication.LoginManager, contract.Song);
			CommentCount = contract.CommentCount;
			CreateDate = contract.CreateDate;
			Deleted = contract.Deleted;
			Draft = contract.Song.Status == EntryStatus.Draft;
			FavoritedTimes = contract.Song.FavoritedTimes;
			Hits = contract.Hits;
			Id = contract.Song.Id;
			IsFavorited = contract.UserRating != SongVoteRating.Nothing;
			LatestComments = contract.LatestComments;
			Length = contract.Song.LengthSeconds;
			LikedTimes = contract.LikeCount;
			Lyrics = contract.LyricsFromParents;
			Name = contract.Song.Name;
			NicoId = contract.Song.NicoId;
			Notes = contract.Notes;
			OriginalVersion = (contract.Song.SongType != SongType.Original ? contract.OriginalVersion : null);
			Pools = contract.Pools;
			RatingScore = contract.Song.RatingScore;
			SongType = contract.Song.SongType;
			Status = contract.Song.Status;
			Tags = contract.Tags;
			UserRating = contract.UserRating;
			WebLinks = contract.WebLinks.ToList();

			Animators = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Animator)).ToArray();
			Performers = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Vocalist)).ToArray();
			Producers = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Producer)).ToArray();
			OtherArtists = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Circle) 
				|| a.Categories.HasFlag(ArtistCategories.Label) 
				|| a.Categories.HasFlag(ArtistCategories.Other)).ToArray();

			var pvs = contract.PVs;

			OriginalPVs = pvs.Where(p => p.PVType == PVType.Original).ToArray();
			OtherPVs = pvs.Where(p => p.PVType != PVType.Original).ToArray();
			PrimaryPV = PVHelper.PrimaryPV(pvs);
			ThumbUrl = VideoServiceHelper.GetThumbUrlPreferNotNico(pvs);

			if (PrimaryPV == null && !string.IsNullOrEmpty(NicoId))
				PrimaryPV = new PVContract { PVId = NicoId, Service = PVService.NicoNicoDouga };

			var nicoPvId = PVHelper.GetNicoId(pvs, NicoId);

			if (!string.IsNullOrEmpty(nicoPvId)) {
				WebLinks.Add(new WebLinkContract(VideoServiceUrlFactory.NicoSound.CreateUrl(nicoPvId), 
					ViewRes.Song.DetailsStrings.CheckNicoSound, WebLinkCategory.Other));
			}

			if (pvs.All(p => p.Service != PVService.Youtube)) {

				var nicoPV = VideoServiceHelper.PrimaryPV(pvs, PVService.NicoNicoDouga);
				var query = (nicoPV != null && !string.IsNullOrEmpty(nicoPV.Name))
					? nicoPV.Name
					: string.Format("{0} {1}", ArtistString, Name);

				WebLinks.Add(new WebLinkContract(string.Format("http://www.youtube.com/results?search_query={0}", query), 
					ViewRes.Song.DetailsStrings.SearchYoutube, WebLinkCategory.Other));

			}

		}

		public string AdditionalNames { get; set; }

		public AlbumContract[] Albums { get; set; }

		[Display(Name = "Alternate versions")]
		public SongContract[] AlternateVersions { get; set; }

		public ArtistForSongContract[] Animators { get; set; }

		public string ArtistString { get; set; }

		public bool CanEdit { get; set; }

		public int CommentCount { get; set; }

		public DateTime CreateDate { get; set; }

		public bool Deleted { get; set; }

		public bool Draft { get; set; }

		public int FavoritedTimes { get; set; }

		public int Hits { get; set; }

		public int Id { get; set; }

		public bool IsFavorited { get; set; }

		public int Length { get; set; }

		public string Json {
			get {
				return JsonHelpers.Serialize(new SongDetailsAjax(this));
			}
		}

		public CommentContract[] LatestComments { get; set; }

		public int LikedTimes { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public string Notes { get; set; }

		public ArtistForSongContract[] OtherArtists { get; set; }

		public PVContract[] OriginalPVs { get; set; }

		[Display(Name = "Original version")]
		public SongContract OriginalVersion { get; set; }

		public PVContract[] OtherPVs { get; set; }

		public ArtistForSongContract[] Performers { get; set; }

		public SongListBaseContract[] Pools { get; set; }

		public PVContract PrimaryPV { get; set; }

		public ArtistForSongContract[] Producers { get; set; }

		public int RatingScore { get; set; }

		public SongType SongType { get; set; }

		public EntryStatus Status { get; set; }

		public TagUsageContract[] Tags { get; set; }

		public string ThumbUrl { get; set; }

		public SongVoteRating UserRating { get; set; }

		public IList<WebLinkContract> WebLinks { get; set; }

	}

	public class SongDetailsAjax {

		public SongDetailsAjax(SongDetails model) {
			Id = model.Id;
			UserRating = model.UserRating;
		}

		public int Id { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongVoteRating UserRating { get; set; }

	}

	[PropertyModelBinder]
	public class SongEdit {

		public SongEdit() {

			ArtistLinks = new List<ArtistForSongContract>();
			Lyrics = new List<LyricsForSongModel>();
			Names = new NameManagerEditContract();
			OriginalVersion = new SongContract();
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
		public NameManagerEditContract Names { get; set; }

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
		public IList<PVContract> PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		[Display(Name = "Entry status")]
		public EntryStatus Status { get; set; }

		public string UpdateNotes { get; set; }

		public Model.Service.EntryValidators.ValidationResult ValidationResult { get; set; }

		[Display(Name = "Web links")]
		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(SongForEditContract song) {

			ParamIs.NotNull(() => song);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			Deleted = song.Deleted;
			Draft = song.Song.Status == EntryStatus.Draft;
			Name = song.Song.Name;
			ValidationResult = song.ValidationResult;

		}

		public SongForEditContract ToContract() {

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
				Names = this.Names,
				Notes = this.Notes ?? string.Empty,
				OriginalVersion = this.OriginalVersion != null && this.OriginalVersion.Id != 0 ? this.OriginalVersion : new SongContract { Id = OriginalVersionId },
				PVs = this.PVs.Select(p => p.NullToEmpty()).ToArray(),
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray(),
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),
			};

		}

	}

	public class LyricsForSongModel {
		
		public LyricsForSongModel() {
			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
		}

		public LyricsForSongModel(LyricsForSongContract contract)
			: this() {
			
			ParamIs.NotNull(() => contract);

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