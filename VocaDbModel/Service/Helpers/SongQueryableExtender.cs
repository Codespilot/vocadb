using System.Linq;
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

	}
}
