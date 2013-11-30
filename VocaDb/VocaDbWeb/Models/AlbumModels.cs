﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models {

	[PropertyModelBinder]
	public class AlbumEdit {

		public AlbumEdit() {
			
			Names = new NameManagerEditContract();
			Pictures = new List<EntryPictureFileContract>();
			PVs = new List<PVContract>();
			Tracks = new List<SongInAlbumEditContract>();
			WebLinks = new List<WebLinkDisplay>();

			AllDiscTypes = EnumVal<DiscType>.Values;

			DiscTypeDescriptions = ViewRes.Album.EditStrings.BaDiscTypeExplanation 
				+ "<br /><br /><ul>" + string.Join("", 
					EnumVal<DiscType>.Values.Where(v => v != DiscType.Unknown).Select(v => string.Format("<li><strong>{0}</strong>: {1}</li>", 
						Translate.DiscTypeName(v), global::Resources.DiscTypeDescriptions.ResourceManager.GetString(v.ToString()))));


		}

		public AlbumEdit(AlbumForEditContract album)
			: this() {

			ParamIs.NotNull(() => album);

			DefaultLanguageSelection = album.TranslatedName.DefaultLanguage;
			Description = album.Description;
			DiscType = album.DiscType;
			Id = album.Id;
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

		public DiscType[] AllDiscTypes { get; set; }

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

		public int Id { get; set; }

		public string Name { get; set; }

		[Display(Name = "Names")]
		public NameManagerEditContract Names { get; set; }

		[Display(Name = "Name in English")]
		public string NameEnglish { get; set; }

		[Display(Name = "Name in Japanese")]
		public string NameJapanese { get; set; }

		[Display(Name = "Name in Romaji")]
		public string NameRomaji { get; set; }

		public IList<EntryPictureFileContract> Pictures { get; set; }

		[Display(Name = "PVs")]
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

		public Model.Service.EntryValidators.ValidationResult ValidationResult { get; set; }

		[Display(Name = "External links")]
		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		public void CopyNonEditableFields(AlbumForEditContract album) {

			ParamIs.NotNull(() => album);

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(MvcApplication.LoginManager);
			ArtistLinks = album.ArtistLinks;
			Deleted = album.Deleted;
			Draft = album.Status == EntryStatus.Draft;
			NameEnglish = album.TranslatedName.English;
			NameJapanese = album.TranslatedName.Japanese;
			NameRomaji = album.TranslatedName.Romaji;
			ValidationResult = album.ValidationResult;

		}

		public AlbumForEditContract ToContract() {

			if (Tracks == null)
				throw new InvalidOperationException("Tracks list was null"); // Shouldn't be null

			return new AlbumForEditContract {
				Description = this.Description ?? string.Empty,
				DiscType = this.DiscType,
				Id = this.Id,
				Name = this.Name,
				Names = this.Names,
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
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray()
			};

		}

	}

	public class AlbumDetails : IEntryImageInformation {

		private readonly string mime;

		public EntryType EntryType {
			get { return EntryType.Album; }
		}

		public string Mime {
			get { return mime; }
		}

		public AlbumDetails() { }

		public AlbumDetails(AlbumDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AdditionalNames = contract.AdditionalNames;
			ArtistString = contract.ArtistString;
			CanEdit = EntryPermissionManager.CanEdit(MvcApplication.LoginManager, contract);
			CommentCount = contract.CommentCount;
			CreateDate = contract.CreateDate;
			Description = contract.Description;
			Deleted = contract.Deleted;
			DiscType = contract.DiscType;
			Draft = contract.Status == EntryStatus.Draft;
			Hits = contract.Hits;
			Id = contract.Id;
			LatestComments = contract.LatestComments;
			MergedTo = contract.MergedTo;
			Name = contract.Name;
			OwnedBy = contract.OwnedCount;
			Pictures = contract.Pictures;
			PVs = contract.PVs;
			RatingAverage = contract.RatingAverage;
			RatingCount = contract.RatingCount;
			Songs = contract.Songs.GroupBy(s => s.DiscNumber).ToArray();
			Status = contract.Status;
			Tags = contract.Tags;
			UserHasAlbum = contract.AlbumForUser != null;
			Version = contract.Version;
			WebLinks = contract.WebLinks;
			WishlistedBy = contract.WishlistCount;
			mime = contract.CoverPictureMime;

			if (contract.AlbumForUser != null) {
				AlbumMediaType = contract.AlbumForUser.MediaType;
				AlbumPurchaseStatus = contract.AlbumForUser.PurchaseStatus;
				CollectionRating = contract.AlbumForUser.Rating;
			}

			if (contract.OriginalRelease != null) {
				CatNum = contract.OriginalRelease.CatNum;
				ReleaseEvent = contract.OriginalRelease.EventName;
				ReleaseDate = contract.OriginalRelease.ReleaseDate;
			}

			var artists = contract.ArtistLinks;

			Circles = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Circle)).ToArray();
			Labels = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Label)).ToArray();
			Producers = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Producer)).ToArray();
			Vocalists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Vocalist)).ToArray();
			OtherArtists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Other) || a.Categories.HasFlag(ArtistCategories.Animator)).ToArray();

			PrimaryPV = PVHelper.PrimaryPV(PVs);

		}

		public string AdditionalNames { get; set; }

		public MediaType AlbumMediaType { get; set; }

		public PurchaseStatus AlbumPurchaseStatus { get; set; }

		public string ArtistString { get; set; }

		public bool CanEdit { get; set; }

		public string CatNum { get; set; }

		public ArtistForAlbumContract[] Circles { get; set; }

		public int CollectionRating { get; set; }

		public int CommentCount { get; set; }

		public DateTime CreateDate { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; }

		public DiscType DiscType { get; set; }

		public bool Draft { get; set; }

		public int Hits { get; set; }

		public int Id { get; set; }

		public ArtistForAlbumContract[] Labels { get; set; }

		public CommentContract[] LatestComments { get; set; }

		public AlbumContract MergedTo { get; set; }

		public string Name { get; set; }

		public ArtistForAlbumContract[] OtherArtists { get; set; }

		public int OwnedBy { get; set; }

		public EntryPictureFileContract[] Pictures { get; set; }

		public PVContract PrimaryPV { get; set; }

		public ArtistForAlbumContract[] Producers { get; set; }

		public PVContract[] PVs { get; set; }

		public double RatingAverage { get; set; }

		public int RatingCount { get; set; }

		public string ReleaseEvent { get; set; }

		public OptionalDateTimeContract ReleaseDate { get; set; }

		public IGrouping<int, SongInAlbumContract>[] Songs { get; set; }

		public EntryStatus Status { get; set; }

		public TagUsageContract[] Tags { get; set; }

		public bool UserHasAlbum { get; set; }

		public int Version { get; set; }

		public ArtistForAlbumContract[] Vocalists { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public int WishlistedBy { get; set; }

	}

}