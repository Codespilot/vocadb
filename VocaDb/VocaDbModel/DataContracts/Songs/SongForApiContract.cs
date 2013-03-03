using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForApiContract {

		public SongForApiContract() { }

		public SongForApiContract(Song song, ContentLanguagePreference languagePreference) {

			Albums = song.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).ToArray();
			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).ToArray();
			Tags = song.Tags.Tags.Select(t => t.Name).ToArray();

			CreateDate = song.CreateDate;
			DefaultName = song.DefaultName;
			DefaultNameLanguage = song.Names.SortNames.DefaultLanguage;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			PVServices = song.PVServices;
			RatingScore = song.RatingScore;
			SongType = song.SongType;
			Status = song.Status;
			ThumbUrl = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);
			Version = song.Version;

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
		public LocalizedStringContract[] Names { get; set; }

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

	}
}
