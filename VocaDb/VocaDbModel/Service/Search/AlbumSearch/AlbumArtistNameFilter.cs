using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumArtistNameFilter : ISearchFilter<Album> {

		private readonly string artistName;

		public AlbumArtistNameFilter(string artistName) {
			this.artistName = artistName;
		}

		public QueryCost Cost {
			get { return QueryCost.High; }
		}

		public IQueryable<Album> Filter(IQueryable<Album> query, IQuerySource session) {

			return query.Where(a => a.AllArtists.Any(u => u.Artist.Names.Names.Any(a2 => a2.Value.Contains(artistName))));

		}

		public IQueryable<Album> Query(IQuerySource session) {

			return session.Query<ArtistName>()
				.Where(an => an.Value.Contains(artistName))
				.SelectMany(an => an.Artist.AllAlbums)
				.Select(an => an.Album)
				.Distinct();

		}
	}
}
