using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models {

	public class SongList {

		public string Filter { get; set; }

	}

	public class SongDetails {

		public SongDetails(SongDetailsContract contract) {

			AdditionalNames = contract.AdditionalNames;
			Albums = contract.Albums;
			Id = contract.Song.Id;
			IsFavorited = contract.IsFavorited;
			Lyrics = contract.Lyrics;
			Name = contract.Song.Name;
			NicoId = contract.Song.NicoId;
			Notes = contract.Notes;
			OtherArtists = contract.Artists.Where(a => !ArtistHelper.VocalistTypes.Contains(a.Artist.ArtistType)).Select(a => a.Artist).ToArray();
			Performers = contract.Artists.Where(a => ArtistHelper.VocalistTypes.Contains(a.Artist.ArtistType)).Select(a => a.Artist).ToArray();
			PVs = contract.PVs;
			SongType = contract.Song.SongType;
			WebLinks = contract.WebLinks;

			PrimaryPV = PVHelper.PrimaryPV(PVs);

			if (PrimaryPV == null && !string.IsNullOrEmpty(NicoId))
				PrimaryPV = new PVContract { PVId = NicoId, Service = PVService.NicoNicoDouga };

		}

		public string AdditionalNames { get; set; }

		public AlbumWithAdditionalNamesContract[] Albums { get; set; }

		public int Id { get; set; }

		public bool IsFavorited { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public string Notes { get; set; }

		public ArtistWithAdditionalNamesContract[] OtherArtists { get; set; }

		public ArtistWithAdditionalNamesContract[] Performers { get; set; }

		public PVContract PrimaryPV { get; set; }

		[Display(Name = "PVs")]
		public PVContract[] PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		[Display(Name = "External links")]
		public WebLinkContract[] WebLinks { get; set; }

	}

	public class SongEdit {

		public SongEdit() {

			Names = new List<LocalizedStringEdit>();
			PVs = new List<PVContract>();
			WebLinks = new List<WebLink>();

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
			OriginalVersion = song.OriginalVersion ?? new SongContract();
			PVs = song.PVs;
			SongType = song.Song.SongType;
			WebLinks = song.WebLinks.Select(w => new WebLink(w)).ToArray();

		}

		public PVType[] AllPVTypes { get; set; }

		public PVService[] AllVideoServices { get; set; }

		public ArtistForSongContract[] ArtistLinks { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public int Id { get; set; }

		[Display(Name = "Lyrics")]
		public LyricsForSongModel[] Lyrics { get; set; }

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
		public SongContract OriginalVersion { get; set; }

		public int OriginalVersionId { get; set; }

		[Display(Name = "PVs")]
		public IList<PVContract> PVs { get; set; }

		[Display(Name = "Song type")]
		public SongType SongType { get; set; }

		[Display(Name = "Web links")]
		public IList<WebLink> WebLinks { get; set; }

		public SongForEditContract ToContract() {

			return new SongForEditContract {
				Song = new SongContract {
					Id = this.Id,
					Name = this.Name,
					SongType = this.SongType
				},
				Names = this.Names.Select(n => n.ToContract()).ToArray(),
				Notes = this.Notes ?? string.Empty,
				OriginalVersion = this.OriginalVersion ?? new SongContract { Id = OriginalVersionId },
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

	public class LyricsEditorModel {

		public LyricsEditorModel() {}

		public LyricsEditorModel(SongEdit model) {

			Id = model.Id;
			Lyrics = model.Lyrics;

		}

		public int Id { get; set; }

		public LyricsForSongModel[] Lyrics { get; set; }

	}

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