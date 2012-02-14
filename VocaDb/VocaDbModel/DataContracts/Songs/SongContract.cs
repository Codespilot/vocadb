using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongContract : IEntryWithStatus {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		EntryType IEntryBase.EntryType {
			get { return EntryType.Song; }
		}

		public SongContract() {}

		public SongContract(Song song, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => song);

			ArtistString = song.ArtistString.GetBestMatch(languagePreference, song.TranslatedName.DefaultLanguage);
			CreateDate = song.CreateDate;
			FavoritedTimes = song.FavoritedTimes;
			Id = song.Id;
			Name = song.TranslatedName[languagePreference];
			NicoId = song.NicoId;
			PVServices = song.PVServices;
			SongType = song.SongType;
			Status = song.Status;
			Version = song.Version;

		}

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public int FavoritedTimes { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public PVServices PVServices { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

}
