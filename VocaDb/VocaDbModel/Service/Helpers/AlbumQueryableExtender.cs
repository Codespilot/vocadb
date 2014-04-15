using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.Helpers {

	public static class AlbumQueryableExtender {

		public static IQueryable<Album> OrderByReleaseDate(this IQueryable<Album> query) {
			
			return query
				.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
				.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);

		} 

		public static IQueryable<Album> OrderBy(this IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference) {
			
			switch (sortRule) {
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.ReleaseDate:
					return criteria
						.WhereHasReleaseDate()
						.OrderByReleaseDate();
				case AlbumSortRule.ReleaseDateWithNulls:
					return criteria.OrderByReleaseDate();
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.RatingTotal:
					return criteria.OrderByDescending(a => a.RatingTotal)
						.ThenByDescending(a => a.RatingAverageInt);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;

		}

		public static IQueryable<Album> WhereHasReleaseDate(this IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

	}
}
