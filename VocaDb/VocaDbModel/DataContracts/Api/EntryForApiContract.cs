using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.DataContracts.Api {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryForApiContract : IEntryWithIntId {

		private EntryForApiContract(IEntryWithNames entry, ContentLanguagePreference languagePreference) {

			EntryType = entry.EntryType;
			Id = entry.Id;

			DefaultName = entry.DefaultName;
			DefaultNameLanguage = entry.Names.SortNames.DefaultLanguage;
			Version = entry.Version;

			if (languagePreference != ContentLanguagePreference.Default) {
				AdditionalNames = entry.Names.GetAdditionalNamesStringForLanguage(languagePreference);
				LocalizedName = entry.Names.SortNames[languagePreference];					
			}

		}

		public EntryForApiContract(Artist artist, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(artist, languagePreference) {

			ArtistType = artist.ArtistType;			
			CreateDate = artist.CreateDate;
			Status = artist.Status;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && artist.Picture != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(artist, artist.Picture.Mime), thumbPersister, ssl);					
			}

		}

		public EntryForApiContract(Album album, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl, 
			EntryOptionalFields includedFields)
			: this(album, languagePreference) {

			CreateDate = album.CreateDate;
			DiscType = album.DiscType;
			Status = album.Status;

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && album.CoverPictureData != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(album, album.CoverPictureData.Mime), thumbPersister, ssl);					
			}

		}

		public EntryForApiContract(Song song, ContentLanguagePreference languagePreference, EntryOptionalFields includedFields)
			: this((IEntryWithNames)song, languagePreference) {
			
			CreateDate = song.CreateDate;
			SongType = song.SongType;
			Status = song.Status;

			var thumb = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

			if (includedFields.HasFlag(EntryOptionalFields.MainPicture) &&!string.IsNullOrEmpty(thumb)) {
				MainPicture = new EntryThumbForApiContract { UrlSmallThumb = thumb, UrlThumb = thumb, UrlTinyThumb = thumb };				
			}

		}

		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		[DataMember(EmitDefaultValue = false)]
		public ArtistType? ArtistType { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string DefaultName { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public DiscType? DiscType { get; set; }

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string LocalizedName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public SongType? SongType { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum EntryOptionalFields {

		None = 0,
		Description = 1,
		MainPicture = 2,
		Names = 4,
		Tags = 8,
		WebLinks = 16

	}

}
