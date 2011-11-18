using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumContract {

		public AlbumContract() { }

		public AlbumContract(Album album, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => album);

			ArtistString = album.ArtistString.GetBestMatch(languagePreference, album.TranslatedName.DefaultLanguage);
			CreateDate = album.CreateDate;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.TranslatedName[languagePreference];
			Version = album.Version;

		}

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

}
