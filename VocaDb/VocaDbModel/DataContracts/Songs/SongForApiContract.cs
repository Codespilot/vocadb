using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForApiContract {

		public SongForApiContract() { }

		public SongForApiContract(Song song, SongMergeRecord mergeRecord, ContentLanguagePreference languagePreference, 
			bool albums = true, bool artists = true, bool names = true, bool pvs = false, bool tags = true, bool webLinks = false) {

			CreateDate = song.CreateDate;
			DefaultName = song.DefaultName;
			DefaultNameLanguage = song.Names.SortNames.DefaultLanguage;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			ThumbUrl = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);
			Version = song.Version;

			if (albums)
				Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).ToArray();

			if (artists)
				Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).ToArray();

			if (names)
				Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (pvs)
				PVs = song.PVs.Select(p => new PVContract(p)).ToArray();

			if (tags)
				Tags = song.Tags.Tags.Select(t => t.Name).ToArray();

			if (webLinks)
				WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		[DataMember]
		public AlbumContract[] Albums { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public int FavoritedTimes { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int MergedTo { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public PVServices PVServices { get; set; }

		[DataMember]
		public int RatingScore { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string[] Tags { get; set; }

		[DataMember]
		public string ThumbUrl { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}
}
