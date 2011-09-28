﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models {

	public class SongList {

		public string Filter { get; set; }

	}

	public class SongEdit {

		public SongEdit(SongForEditContract song) {

			Artists = song.Artists;
			Id = song.Song.Id;
			Name = song.Song.Name;
			Lyrics = song.Lyrics;
			Names = song.Names;
			WebLinks = song.WebLinks;

			AllLanguages = EnumVal<ContentLanguageSelection>.Values;
			AllLanguagesJson = JsonConvert.SerializeObject(AllLanguages);

		}

		public ContentLanguageSelection[] AllLanguages { get; set; }

		public string AllLanguagesJson { get; set; }

		public ArtistForSongContract[] Artists { get; protected set; }

		[Display(Name = "Default language")]
		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public int Id { get; set; }

		public LyricsForSongContract[] Lyrics { get; protected set; }

		public string Name { get; protected set; }

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

		[Display(Name = "NicoNicoDouga ID")]
		public string NicoId { get; protected set; }

		[Display(Name = "Web links")]
		public WebLinkContract[] WebLinks { get; set; }

	}

}