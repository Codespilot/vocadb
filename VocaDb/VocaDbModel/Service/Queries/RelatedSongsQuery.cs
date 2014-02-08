using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class RelatedSongsQuery {

		private readonly IRepositoryContext<Song> ctx;

		private Artist[] GetMainArtists(Song song, IList<IArtistWithSupport> creditableArtists) {

			return ArtistHelper.GetProducers(creditableArtists, SongHelper.IsAnimation(song.SongType)).Select(a => a.Artist).ToArray();

		}

		public RelatedSongsQuery(IRepositoryContext<Song> ctx) {

			ParamIs.NotNull(() => ctx);

			this.ctx = ctx;

		}

		public RelatedSongs GetRelatedSongs(Song song) {

			ParamIs.NotNull(() => song);

			var songs = new RelatedSongs();
			var songId = song.Id;
			var creditableArtists = song.Artists.Where(a => a.Artist != null && !a.IsSupport).ToArray();

			var mainArtists = GetMainArtists(song, creditableArtists);

			if (mainArtists != null && mainArtists.Any()) {

				var mainArtistIds = mainArtists.Select(a => a.Id).ToArray();
				var songsByMainArtists = ctx.Query()
					.Where(al => 
						al.Id != songId
						&& !al.Deleted 
						&& al.AllArtists.Any(a => 
							!a.Artist.Deleted
							//&& (al.ArtistString.Default != ArtistHelper.VariousArtists)
							&& !a.IsSupport 
							&& mainArtistIds.Contains(a.Artist.Id)))
					.OrderBy(a => a.RatingScore)
					.Distinct()
					.Take(20)
					.ToArray();

				songs.ArtistMatches = songsByMainArtists;

			}

			if (song.Tags.Tags.Any()) {

				// Take top 5 tags
				var tagNames = song.Tags.Usages.OrderByDescending(u => u.Count).Take(5).Select(t => t.Tag.Name).ToArray();
				var otherSongIds = songs.ArtistMatches.Select(a => a.Id).ToArray();

				var songsWithTags =
					ctx.Query().Where(al => 
						al.Id != songId
						&& !otherSongIds.Contains(al.Id) 
						&& !al.Deleted 
						&& al.Tags.Usages.Any(t => tagNames.Contains(t.Tag.Name)))
					.OrderBy(a => a.RatingScore)
					.Take(16)
					.ToArray();

				songs.TagMatches = songsWithTags;

			}

			return songs;

		}

	}

	public class RelatedSongs {

		public RelatedSongs() {
			ArtistMatches = new Song[0];
			TagMatches = new Song[0];
		}

		public Song[] ArtistMatches { get; set; }

		public Song[] TagMatches { get; set; }

	}
}
