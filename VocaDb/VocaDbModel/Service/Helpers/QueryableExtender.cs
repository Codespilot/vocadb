using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Helpers {

	public static class QueryableExtender {

		public static IQueryable<ArtistName> AddArtistNameFilter(this IQueryable<ArtistName> query, string originalQuery, 
			string canonizedName, NameMatchMode matchMode, string[] words = null) {

			canonizedName = canonizedName ?? ArtistHelper.GetCanonizedName(originalQuery);

			if (FindHelpers.ExactMatch(originalQuery, matchMode)) {

				return query.Where(m => m.Value == canonizedName 
					|| m.Value == string.Format("{0}P", canonizedName) 
					|| m.Value == string.Format("{0}-P", canonizedName));

			} else {

				return FindHelpers.AddEntryNameFilter(query, canonizedName, matchMode, words);

			}

		}

		public static IQueryable<T> AddEntryNameFilter<T>(this IQueryable<T> query, string nameFilter, NameMatchMode matchMode)
			where T : LocalizedString {

			return FindHelpers.AddEntryNameFilter(query, nameFilter, matchMode);

		}

		public static IQueryable<T> AddNameOrder<T>(this IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IEntryWithNames {

			return FindHelpers.AddNameOrder(criteria, languagePreference);

		}

		public static IQueryable<AlbumForUser> AddNameOrder(this IQueryable<AlbumForUser> criteria, ContentLanguagePreference languagePreference) {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Album.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Album.Names.SortNames.English);
				default:
					return criteria.OrderBy(e => e.Album.Names.SortNames.Romaji);
			}

		}
	}

}
