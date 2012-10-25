using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Helpers {

	public static class ArtistQueryableExtender {

		public static IQueryable<ArtistName> FilterByArtistName(this IQueryable<ArtistName> query, string originalQuery,
			string canonizedName = null, NameMatchMode matchMode = NameMatchMode.Auto, string[] words = null) {

			canonizedName = canonizedName ?? ArtistHelper.GetCanonizedName(originalQuery);

			if (FindHelpers.ExactMatch(canonizedName, matchMode)) {

				return query.Where(m => m.Value == canonizedName
					|| m.Value == string.Format("{0}P", canonizedName)
					|| m.Value == string.Format("{0}-P", canonizedName));

			} else {

				return FindHelpers.AddEntryNameFilter(query, canonizedName, matchMode, words);

			}

		}

		public static IQueryable<ArtistName> FilterByArtistType(this IQueryable<ArtistName> queryable, ArtistType[] types) {

			if (types == null || !types.Any())
				return queryable;

			return queryable.Where(n => types.Contains(n.Artist.ArtistType));

		}

		public static IQueryable<Artist> OrderByArtistRule(this IQueryable<Artist> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case ArtistSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case ArtistSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
			}

			return criteria;

		}

	}
}
