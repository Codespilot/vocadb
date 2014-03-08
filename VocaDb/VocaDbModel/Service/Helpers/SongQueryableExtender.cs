using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service.Helpers {

	public static class SongQueryableExtender {

		public static IQueryable<Song> AddOrder(this IQueryable<Song> criteria, SongSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case SongSortRule.Name:
					return criteria.AddNameOrder(languagePreference);
				case SongSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case SongSortRule.FavoritedTimes:
					return criteria.OrderByDescending(a => a.FavoritedTimes);
				case SongSortRule.RatingScore:
					return criteria.OrderByDescending(a => a.RatingScore);
			}

			return criteria;

		}

		public static IQueryable<T> AddSongOrder<T>(this IQueryable<T> criteria, SongSortRule sortRule, ContentLanguagePreference languagePreference)
			where T : ISongLink {

			switch (sortRule) {
				case SongSortRule.Name:
					return criteria.AddSongNameOrder(languagePreference);
				case SongSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.Song.CreateDate);
				case SongSortRule.FavoritedTimes:
					return criteria.OrderByDescending(a => a.Song.FavoritedTimes);
				case SongSortRule.RatingScore:
					return criteria.OrderByDescending(a => a.Song.RatingScore);
			}

			return criteria;

		}

		public static IQueryable<Song> WhereArtistHasTag(this IQueryable<Song> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.AllArtists.Any(a => a.Artist.Tags.Usages.Any(u => u.Tag.Name == tagName)));

		}

		public static IQueryable<Song> WhereDraftsOnly(this IQueryable<Song> query, bool draftsOnly) {

			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);

		}

		/*
		/// <summary>
		/// Filters a song query by a list of artist Ids.
		/// </summary>
		public static IQueryable<Song> WhereHasArtist(this IQueryable<Song> query, int[] artistIds) {

			if (artistIds == null || artistIds.Length == 0)
				return query;

			if (artistIds.Length == 1)
				return WhereHasArtist(query, artistIds.First());

			// TODO: should change to AND
			return query.Where(s => s.AllArtists.Any(a => artistIds.Contains(a.Artist.Id)));

		}*/

		/// <summary>
		/// Filters a song query by a single artist Id.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="artistId">ID of the artist being filtered. If 0, no filtering is done.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Song> WhereHasArtist(this IQueryable<Song> query, int artistId) {

			if (artistId == 0)
				return query;

			return query.Where(s => s.AllArtists.Any(a => a.Artist.Id == artistId));

		}

		/// <summary>
		/// Filters a song query by a name query.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Song> WhereHasName(this IQueryable<Song> query, string nameFilter, 
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

		public static IQueryable<Song> WhereHasNicoId(this IQueryable<Song> query, string nicoId) {

			if (string.IsNullOrEmpty(nicoId))
				return query;

			return query.Where(s => s.NicoId == nicoId);

		}

		public static IQueryable<Song> WhereHasTag(this IQueryable<Song> query, string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.Tags.Usages.Any(a => a.Tag.Name == tagName));

		}

		public static IQueryable<Song> WhereHasType(this IQueryable<Song> query, SongType[] songTypes) {

			if (!songTypes.Any())
				return query;

			return query.Where(m => songTypes.Contains(m.SongType));

		}

	}
}
