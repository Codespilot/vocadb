using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForApiContract {

		public AlbumForApiContract() { }

		public AlbumForApiContract(Album album, AlbumMergeRecord mergeRecord, ContentLanguagePreference languagePreference, 
			bool artists = true, bool names = true, bool tags = true, bool webLinks = false) {

			CatalogNumber = album.OriginalRelease != null ? album.OriginalRelease.CatNum : null;
			CreateDate = album.CreateDate;
			DefaultName = album.DefaultName;
			DefaultNameLanguage = album.Names.SortNames.DefaultLanguage;
			DiscType = album.DiscType;
			Id = album.Id;
			RatingAverage = album.RatingAverage;
			RatingCount = album.RatingCount;
			ReleaseDate = new OptionalDateTimeContract(album.OriginalReleaseDate);
			ReleaseEvent = album.OriginalReleaseEventName;
			Status = album.Status;
			Version = album.Version;

			if (artists)
				Artists = album.Artists.Select(a => new ArtistForAlbumForApiContract(a, languagePreference)).ToArray();

			if (names)
				Names = album.Names.Select(n => new LocalizedStringContract(n)).ToArray();

			if (tags)
				Tags = album.Tags.Tags.Select(t => t.Name).ToArray();

			if (webLinks)
				WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		[DataMember]
		public ArtistForAlbumForApiContract[] Artists { get; set; }

		[DataMember]
		public string CatalogNumber { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int MergedTo { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public double RatingAverage { get; set; }

		[DataMember]
		public int RatingCount { get; set; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		[DataMember]
		public string ReleaseEvent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string[] Tags { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
