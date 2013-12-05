﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongDetailsContract {

		public SongDetailsContract() {}

		public SongDetailsContract(Song song, ContentLanguagePreference languagePreference) {

			Song = new SongContract(song, languagePreference);

			AdditionalNames = string.Join(", ", song.AllNames.Where(n => n != Song.Name).Distinct());
			Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).Distinct().OrderBy(a => a.Name).ToArray();
			AlternateVersions = song.AlternateVersions.Select(s => new SongContract(s, languagePreference)).ToArray();
			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			ArtistString = song.ArtistString[languagePreference];
			CreateDate = song.CreateDate;
			Deleted = song.Deleted;
			LikeCount = song.UserFavorites.Count(f => f.Rating == SongVoteRating.Like);
			LyricsFromParents = song.LyricsFromParents.Select(l => new LyricsForSongContract(l)).ToArray();
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null && !song.OriginalVersion.Deleted ? new SongContract(song.OriginalVersion, languagePreference) : null);

			// TODO (PERF): this might be handled through a special query if the list is long
			Pools =
				song.ListLinks
				.Where(l => l.List.FeaturedCategory == SongListFeaturedCategory.Pools)
				.OrderBy(l => l.List.Name).Take(3)
				.Select(l => new SongListBaseContract(l.List))
				.ToArray();

			ListCount = song.ListLinks.Count;

			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			Tags = song.Tags.Usages.Select(u => new TagUsageContract(u)).OrderByDescending(t => t.Count).ToArray();
			TranslatedName = new TranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public AlbumContract[] Albums { get; set; }

		[DataMember]
		public SongContract[] AlternateVersions { get; set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int Hits { get; set; }

		[DataMember]
		public CommentContract[] LatestComments { get; set; }

		[DataMember]
		public int LikeCount { get; set; }

		[DataMember]
		public int ListCount { get; set; }

		[DataMember]
		public LyricsForSongContract[] LyricsFromParents { get; set; }

		[DataMember]
		public SongContract MergedTo { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public SongContract OriginalVersion { get; set; }

		[DataMember]
		public SongListBaseContract[] Pools { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongContract Song { get; set; }

		[DataMember]
		public TagUsageContract[] Tags { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public SongVoteRating UserRating { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
