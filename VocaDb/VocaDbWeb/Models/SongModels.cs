using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
			OtherArtists = contract.Artists.Where(a => a.Artist.ArtistType != ArtistType.Performer).Select(a => a.Artist).ToArray();
			Performers = contract.Artists.Where(a => a.Artist.ArtistType == ArtistType.Performer).Select(a => a.Artist).ToArray();

		}

		public string AdditionalNames { get; set; }

		public AlbumContract[] Albums { get; set; }

		public int Id { get; set; }

		public bool IsFavorited { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public ArtistContract[] OtherArtists { get; set; }

		public ArtistContract[] Performers { get; set; }

	}

	public class SongEdit {

		public SongEdit() {}

		public SongEdit(SongForEditContract song) {

			ParamIs.NotNull(() => song);

			ArtistLinks = song.Artists;
			Id = song.Song.Id;
			Lyrics = song.Lyrics.Select(l => new LyricsForSongModel(l)).ToArray();
			Name = song.Song.Name;
			NameEnglish = song.TranslatedName.English;
			NameJapanese = song.TranslatedName.Japanese;
			NameRomaji = song.TranslatedName.Romaji;
			Names = song.Names.Select(l => new LocalizedStringEdit(l)).ToArray();
			NicoId = song.Song.NicoId;
			WebLinks = song.WebLinks;

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		public ArtistForSongContract[] ArtistLinks { get; set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public int Id { get; set; }

		[Display(Name = "Lyrics")]
		public LyricsForSongModel[] Lyrics { get; set; }

		public string Name { get; protected set; }

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

		[Display(Name = "NicoNicoDouga ID")]
		public string NicoId { get; set; }

		[Display(Name = "Web links")]
		public WebLinkContract[] WebLinks { get; set; }

		public SongForEditContract ToContract() {

			return new SongForEditContract {
				Song = new SongContract {
					Id = this.Id,
					Name = this.Name,
					NicoId = this.NicoId
				},
				TranslatedName = new TranslatedStringContract(
					NameEnglish, NameJapanese, NameRomaji, DefaultLanguageSelection),				
			};

		}

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