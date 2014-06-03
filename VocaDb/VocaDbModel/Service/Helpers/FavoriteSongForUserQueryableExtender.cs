using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public static class FavoriteSongForUserQueryableExtender {

		public static IQueryable<FavoriteSongForUser> WhereSongHasArtist(this IQueryable<FavoriteSongForUser> query, int artistId) {
			
			if (artistId == 0)
				return query;

			return query.Where(s => s.Song.AllArtists.Any(a => a.Artist.Id == artistId));

		}

		public static IQueryable<FavoriteSongForUser> WhereHasRating(this IQueryable<FavoriteSongForUser> query, SongVoteRating rating) {

			if (rating == SongVoteRating.Nothing)
				return query;

			return query.Where(q => q.Rating == rating);

		}

	}
}
