using System.Runtime.Serialization;
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
			AdditionalNames = entry.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			LocalizedName = entry.Names.SortNames[languagePreference];				

		}

		public EntryForApiContract(Artist artist, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl)
			: this(artist, languagePreference) {

			if (artist.Picture != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(artist, artist.Picture.Mime), thumbPersister, ssl);					
			}

		}

		public EntryForApiContract(Album album, ContentLanguagePreference languagePreference, IEntryThumbPersister thumbPersister, bool ssl)
			: this(album, languagePreference) {

			if (album.CoverPictureData != null) {
				MainPicture = new EntryThumbForApiContract(new EntryThumb(album, album.CoverPictureData.Mime), thumbPersister, ssl);					
			}

		}

		public EntryForApiContract(Song song, ContentLanguagePreference languagePreference)
			: this((IEntryWithNames)song, languagePreference) {
			
			var thumb = VideoServiceHelper.GetThumbUrl(song.PVs.PVs);

			if (!string.IsNullOrEmpty(thumb)) {
				MainPicture = new EntryThumbForApiContract { UrlSmallThumb = thumb, UrlThumb = thumb, UrlTinyThumb = thumb };				
			}

		}

		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set;}

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string LocalizedName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

	}
}
