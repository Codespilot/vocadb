﻿using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistContract : IEntryWithStatus, IEntryImageInformation {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		EntryType IEntryBase.EntryType {
			get { return EntryType.Artist; }
		}

		EntryType IEntryImageInformation.EntryType {
			get { return EntryType.Artist; }
		}

		string IEntryImageInformation.Mime {
			get { return PictureMime; }
		}

		public ArtistContract() {}

		public ArtistContract(Artist artist, ContentLanguagePreference preference) {

			ParamIs.NotNull(() => artist);

			AdditionalNames = artist.Names.GetAdditionalNamesStringForLanguage(preference);
			ArtistType = artist.ArtistType;
			Deleted = artist.Deleted;
			Id = artist.Id;
			Name = artist.TranslatedName[preference];
			PictureMime = artist.PictureMime;
			Status = artist.Status;
			Version = artist.Version;

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistType ArtistType { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PictureMime { get; set;}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

		public override string ToString() {
			return string.Format("Artist {0} [{1}]", Name, Id);
		}

	}

}
