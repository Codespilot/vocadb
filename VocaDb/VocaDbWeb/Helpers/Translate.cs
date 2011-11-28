using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources;
using VocaDb.Model.Domain.Globalization;
using Resources;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Helpers.Support;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static readonly TranslateableEnum<UserGroupId> UserGroups =
			new TranslateableEnum<UserGroupId>(() => UserGroupNames.ResourceManager);

		public static string AlbumEditableField(AlbumEditableFields field) {

			return AlbumEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string AlbumArchiveReason(AlbumArchiveReason reason) {

			return AlbumArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string ArtistEditableField(ArtistEditableFields field) {

			return ArtistEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string ArtistArchiveReason(ArtistArchiveReason reason) {

			return ArtistArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string ArtistTypeName(ArtistType artistType) {

			return ArtistTypeNames.ResourceManager.GetString(artistType.ToString());

		}

		public static string ContentLanguagePreferenceName(ContentLanguagePreference languagePreference) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languagePreference.ToString());

		}

		public static string ContentLanguageSelectionName(ContentLanguageSelection languageSelection) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languageSelection.ToString());

		}

		public static string DiscTypeName(DiscType discType) {

			return DiscTypeNames.ResourceManager.GetString(discType.ToString());

		}

		public static string EmailOptions(UserEmailOptions emailOptions) {

			return UserEmailOptionsNames.ResourceManager.GetString(emailOptions.ToString());

		}

		public static string MediaTypeName(MediaType mediaType) {

			switch (mediaType) {
				case MediaType.DigitalDownload:
					return "Digital download";

				case MediaType.PhysicalDisc:
					return "Physical disc";

				default:
					return "Other";
			}

		}

		public static string SongArchiveReason(SongArchiveReason reason) {

			return SongArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string SongEditableField(SongEditableFields field) {

			return SongEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

	}

}