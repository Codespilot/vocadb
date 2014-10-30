using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class QueryableForSongLinkExtender {

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

		public static IQueryable<T> WhereSongHasArtist<T>(this IQueryable<T> query, int artistId)
			where T : ISongLink {
			
			if (artistId == 0)
				return query;

			return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId));

		}

		public static IQueryable<T> WhereSongIsInList<T>(this IQueryable<T> query, int listId)
			where T : ISongLink {
			
			if (listId == 0)
				return query;

			return query.Where(s => s.Song.ListLinks.Any(l => l.List.Id == listId));

		}

		public static IQueryable<T> WhereSongHasTag<T>(this IQueryable<T> query, string tag)
			where T : ISongLink {
			
			if (string.IsNullOrEmpty(tag))
				return query;

			return query.Where(s => s.Song.Tags.Usages.Any(t => t.Tag.Name == tag));

		}

	}

}
