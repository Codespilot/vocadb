using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class RelatedAlbumsQuery {

		private readonly IRepositoryContext<Album> ctx;

		private Artist[] GetMainArtists(Album album, IList<IArtistWithSupport> creditableArtists) {

			// "Various artists" albums will be treated as collaboration albums where only the circle/label is searched.
			if (album.ArtistString.Default == ArtistHelper.VariousArtists) {

				var circles = creditableArtists.Where(a => a.Artist != null && ArtistHelper.GetCategories(a).HasFlag(ArtistCategories.Circle)).Select(a => a.Artist).ToArray();

				// No circles found, try labels
				if (!circles.Any()) {
					circles = creditableArtists.Where(a => a.Artist != null && ArtistHelper.GetCategories(a).HasFlag(ArtistCategories.Label)).Select(a => a.Artist).ToArray();
				}

				return circles;

			}

			return ArtistHelper.GetProducers(creditableArtists, AlbumHelper.IsAnimation(album.DiscType)).Select(a => a.Artist).ToArray();

		}

		public RelatedAlbumsQuery(IRepositoryContext<Album> ctx) {

			ParamIs.NotNull(() => ctx);

			this.ctx = ctx;

		}

		public RelatedAlbums GetRelatedAlbums(Album album) {

			ParamIs.NotNull(() => album);

			var albums = new RelatedAlbums();
			var albumId = album.Id;
			var creditableArtists = album.Artists.Where(a => a.Artist != null && !a.IsSupport).ToArray();

			var mainArtists = GetMainArtists(album, creditableArtists);

			if (mainArtists != null && mainArtists.Any()) {

				var mainArtistIds = mainArtists.Select(a => a.Id).ToArray();
				var albumsByMainArtists = ctx.Query()
					.Where(al => 
						al.Id != albumId
						&& !al.Deleted 
						//&& al.ArtistString.Default != ArtistHelper.VariousArtists 
						&& al.AllArtists.Any(a => 
							!a.Artist.Deleted
							&& (a.Artist.ArtistType == ArtistType.Circle || a.Artist.ArtistType == ArtistType.Label || al.ArtistString.Default != ArtistHelper.VariousArtists)
							&& !a.IsSupport 
							&& mainArtistIds.Contains(a.Artist.Id)))
					.OrderBy(a => a.RatingTotal)
					.Distinct()
					.Take(30)
					.ToArray();

				albums.ArtistMatches = albumsByMainArtists;

			}

			if (album.Tags.Tags.Any()) {

				// Take top 5 tags
				var tagNames = album.Tags.Usages.OrderByDescending(u => u.Count).Take(5).Select(t => t.Tag.Name).ToArray();
				var otherAlbumIds = albums.ArtistMatches.Select(a => a.Id).ToArray();

				var albumsWithTags =
					ctx.Query().Where(al => 
						al.Id != albumId
						&& !otherAlbumIds.Contains(al.Id) 
						&& !al.Deleted 
						&& al.Tags.Usages.Any(t => tagNames.Contains(t.Tag.Name)))
					.OrderBy(a => a.RatingTotal)
					.Take(20)
					.ToArray();

				albums.TagMatches = albumsWithTags;

			}

			return albums;

		}

	}

	public class RelatedAlbums {

		public RelatedAlbums() {
			ArtistMatches = new Album[0];
			TagMatches = new Album[0];
		}

		public Album[] ArtistMatches { get; set; }

		public Album[] TagMatches { get; set; }

	}

}
