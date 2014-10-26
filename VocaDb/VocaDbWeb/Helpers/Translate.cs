﻿using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Resources;
using VocaDb.Model.Domain.Globalization;
using Resources;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Resources.Albums;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers.Support;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers {

	public static class Translate {

		public static readonly TranslateableEnum<PurchaseStatus> AlbumCollectionStatusNames =
			new TranslateableEnum<PurchaseStatus>(() => global::Resources.AlbumCollectionStatusNames.ResourceManager);

		public static readonly TranslateableEnum<MediaType> AlbumMediaTypeNames =
			new TranslateableEnum<MediaType>(() => global::Resources.AlbumMediaTypeNames.ResourceManager);

		public static readonly TranslateableEnum<AlbumReportType> AlbumReportTypeNames =
			new TranslateableEnum<AlbumReportType>(() => global::Resources.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<AlbumSortRule> AlbumSortRuleNames =
			new TranslateableEnum<AlbumSortRule>(() => global::Resources.AlbumSortRuleNames.ResourceManager, new[] {
				AlbumSortRule.Name, AlbumSortRule.AdditionDate, AlbumSortRule.ReleaseDate, AlbumSortRule.RatingAverage, AlbumSortRule.RatingTotal,
				AlbumSortRule.CollectionCount
			});

		public static readonly TranslateableEnum<ArtistReportType> ArtistReportTypeNames =
			new TranslateableEnum<ArtistReportType>(() => global::Resources.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<ArtistRoles> ArtistRoleNames =
			new TranslateableEnum<ArtistRoles>(() => global::Resources.ArtistRoleNames.ResourceManager);

		public static readonly TranslateableEnum<ArtistSortRule> ArtistSortRuleNames =
			new TranslateableEnum<ArtistSortRule>(() => global::Resources.ArtistSortRuleNames.ResourceManager, new[] {
				ArtistSortRule.Name, ArtistSortRule.AdditionDate, ArtistSortRule.AdditionDateAsc,
				ArtistSortRule.SongCount, ArtistSortRule.SongRating
			});

		public static TranslateableEnum<ArtistType> ArtistTypeNames {
			get {
				return new TranslateableEnum<ArtistType>(() => Model.Resources.ArtistTypeNames.ResourceManager);
			}
		}			

		public static TranslateableEnum<DiscType> DiscTypeNames {
			get {
				return new TranslateableEnum<DiscType>(() => Model.Resources.Albums.DiscTypeNames.ResourceManager);
			}
		}			

		public static readonly TranslateableEnum<EntryEditEvent> EntryEditEventNames =
			new TranslateableEnum<EntryEditEvent>(() => global::Resources.EntryEditEventNames.ResourceManager);

		public static readonly TranslateableEnum<EntryStatus> EntryStatusNames =
			new TranslateableEnum<EntryStatus>(() => global::Resources.EntryStatusNames.ResourceManager);

		public static readonly TranslateableEnum<EntryType> EntryTypeNames =
			new TranslateableEnum<EntryType>(() => global::Resources.EntryTypeNames.ResourceManager);

		public static readonly TranslateableEnum<PVType> PVTypeDescriptions =
			new TranslateableEnum<PVType>(() => global::Resources.PVTypeDescriptions.ResourceManager);

		public static readonly TranslateableEnum<PVType> PVTypeNames =
			new TranslateableEnum<PVType>(() => global::Resources.PVTypeNames.ResourceManager);

		public static readonly TranslateableEnum<ReleaseEventEditableFields> ReleaseEventEditableFieldNames =
			new TranslateableEnum<ReleaseEventEditableFields>(() => global::Resources.ReleaseEventEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<SongEditableFields> SongEditableFieldNames =
			new TranslateableEnum<SongEditableFields>(() => global::Resources.SongEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<SongListFeaturedCategory> SongListFeaturedCategoryNames =
			new TranslateableEnum<SongListFeaturedCategory>(() => global::Resources.SongListFeaturedCategoryNames.ResourceManager);

		public static readonly TranslateableEnum<SongReportType> SongReportTypeNames =
			new TranslateableEnum<SongReportType>(() => global::Resources.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<SongSortRule> SongSortRuleNames =
			new TranslateableEnum<SongSortRule>(() => global::Resources.SongSortRuleNames.ResourceManager, new[] { SongSortRule.Name, SongSortRule.AdditionDate, SongSortRule.RatingScore, SongSortRule.FavoritedTimes,  });

		public static readonly TranslateableEnum<SongType> SongTypeNames =
			new TranslateableEnum<SongType>(() => Model.Resources.Songs.SongTypeNames.ResourceManager);

		public static readonly TranslateableEnum<SongVoteRating> SongVoteRatingNames =
			new TranslateableEnum<SongVoteRating>(() => global::Resources.SongVoteRatingNames.ResourceManager);

		public static readonly TranslateableEnum<TagEditableFields> TagEditableFieldNames =
			new TranslateableEnum<TagEditableFields>(() => global::Resources.TagEditableFieldNames.ResourceManager);

		public static readonly TranslateableEnum<UserGroupId> UserGroups =
			new TranslateableEnum<UserGroupId>(() => UserGroupNames.ResourceManager);

		public static readonly TranslateableEnum<UserReportType> UserReportTypeNames =
			new TranslateableEnum<UserReportType>(() => global::Resources.EntryReportTypeNames.ResourceManager);

		public static readonly TranslateableEnum<WebLinkCategory> WebLinkCategoryNames =
			new TranslateableEnum<WebLinkCategory>(() => global::Resources.WebLinkCategoryNames.ResourceManager);

		public static string AlbumEditableField(AlbumEditableFields field) {

			return AlbumEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string AlbumArchiveReason(AlbumArchiveReason reason) {

			return AlbumArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string AllPermissionTokenNames(IEnumerable<PermissionToken> tokens) {

			return string.Join(", ", tokens.Select(PermissionTokenName));

		}

		public static string ArtistEditableField(ArtistEditableFields field) {

			return ArtistEditableFieldNames.ResourceManager.GetString(field.ToString());

		}

		public static string ArtistArchiveReason(ArtistArchiveReason reason) {

			return ArtistArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string ArtistTypeName(ArtistType artistType) {

			return Model.Resources.ArtistTypeNames.ResourceManager.GetString(artistType.ToString());

		}

		public static string ContentLanguagePreferenceName(ContentLanguagePreference languagePreference) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languagePreference.ToString());

		}

		public static string ContentLanguageSelectionName(ContentLanguageSelection languageSelection) {

			return ContentLanguageSelectionNames.ResourceManager.GetString(languageSelection.ToString());

		}

		public static string DiscTypeName(DiscType discType) {

			return Model.Resources.Albums.DiscTypeNames.ResourceManager.GetString(discType.ToString());

		}

		public static string EmailOptions(UserEmailOptions emailOptions) {

			return UserEmailOptionsNames.ResourceManager.GetString(emailOptions.ToString());

		}

		public static string SongArchiveReason(SongArchiveReason reason) {

			return SongArchiveReasonNames.ResourceManager.GetString(reason.ToString());

		}

		public static string SongEditableField(SongEditableFields field) {

			return SongEditableFieldNames[field];

		}

		public static string PermissionTokenName(PermissionToken token) {

			return PermissionTokenNames.ResourceManager.GetString(token.Name) ?? token.Name;

		}

		public static string PermissionTokenName(PermissionTokenContract token) {

			return PermissionTokenNames.ResourceManager.GetString(PermissionToken.GetNameById(token.Id)) ?? token.Name;
			 
		}

	}

}