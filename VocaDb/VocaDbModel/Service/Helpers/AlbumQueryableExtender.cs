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

		// TODO: should be combined with common name query somehow, but NH is making it hard
		/// <summary>
		/// Filters an artist query by a name query.
		/// </summary>
		/// <param name="query">Artist query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Album> WhereHasName(this IQueryable<Album> query, string nameFilter, 
			NameMatchMode matchMode, string[] words = null) {

			if (string.IsNullOrEmpty(nameFilter))
				return query;

			switch (FindHelpers.GetMatchMode(nameFilter, matchMode)) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Names.Names.Any(n => n.Value == nameFilter));

				case NameMatchMode.Partial:
					return query.Where(m => m.Names.Names.Any(n => n.Value.Contains(nameFilter)));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Names.Names.Any(n => n.Value.StartsWith(nameFilter)));

				case NameMatchMode.Words:
					words = words ?? FindHelpers.GetQueryWords(nameFilter);

					switch (words.Length) {
						case 1:
							query = query.Where(q => q.Names.Names.Any(n => n.Value.Contains(words[0])));
							break;
						case 2:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
							);
							break;
						case 3:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
							);
							break;
						case 4:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
							);
							break;
						case 5:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
							);
							break;
						case 6:
							query = query.Where(q => 
								q.Names.Names.Any(n => n.Value.Contains(words[0]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[1]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[2]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[3]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[4]))
								&& q.Names.Names.Any(n => n.Value.Contains(words[5]))
							);
							break;
					}
					return query;

			}

			return query;

		}

		public static IQueryable<Album> WhereHasReleaseDate(this IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

		public static IQueryable<Album> WhereHasTag(this IQueryable<Album> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Name == tagName));

		}

	}
}
