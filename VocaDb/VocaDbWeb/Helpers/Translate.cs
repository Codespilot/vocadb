using System.Linq;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources;
using VocaDb.Model.Domain.Globalization;
using Resources;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Helpers.Support;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static readonly TranslateableEnum<PurchaseStatus> AlbumCollectionStatusNames =
			new TranslateableEnum<PurchaseStatus>(() => Resources.AlbumCollectionStatusNames.ResourceManager);

		public static readonly TranslateableEnum<MediaType> AlbumMediaTypeNames =
			new TranslateableEnum<MediaType>(() => Resources.AlbumMediaTypeNames.ResourceManager);

		public static readonly TranslateableEnum<EntryStatus> EntryStatusNames =
			new TranslateableEnum<EntryStatus>(() => Resources.EntryStatusNames.ResourceManager);

		public static readonly TranslateableEnum<PVType> PVTypeNames =
			new TranslateableEnum<PVType>(() => Resources.PVTypeNames.ResourceManager);

		public static readonly TranslateableEnum<SongListFeaturedCategory> SongListFeaturedCategoryNames =
			new TranslateableEnum<SongListFeaturedCategory>(() => Resources.SongListFeaturedCategoryNames.ResourceManager);

		public static readonly TranslateableEnum<UserGroupId> UserGroups =
			new TranslateableEnum<UserGroupId>(() => UserGroupNames.ResourceManager);

		public static string AlbumEditableField(AlbumEditableFields field) {

			return AlbumEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string AlbumArchiveReason(AlbumArchiveReason reason) {

			return AlbumArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string AllPermissionTokenNames(PermissionCollection collection) {

			return string.Join(", ", collection.PermissionTokens.Select(PermissionTokenName));

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

		public static string SongArchiveReason(SongArchiveReason reason) {

			return SongArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string SongEditableField(SongEditableFields field) {

			return SongEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string PermissionTokenName(PermissionToken token) {

			return PermissionTokenNames.ResourceManager.GetString(PermissionToken.GetNameById(token.Id));

		}

		public static string PermissionTokenName(PermissionTokenContract token) {

			return PermissionTokenNames.ResourceManager.GetString(PermissionToken.GetNameById(token.Id));
			 
		}

	}

}