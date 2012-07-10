using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumArtistFilter : ISearchFilter<Album> {

		private readonly string artistName;

		public AlbumArtistFilter(string artistName) {
			this.artistName = artistName;
		}

		public QueryCost Cost {
			get { return QueryCost.High; }
		}

		public void FilterResults(List<Album> albums, ISession session) {

			albums.RemoveAll(a => !(
				a.Artists.Any(r => r.Artist.Names.Any(n => n.Value.IndexOf(artistName, StringComparison.InvariantCultureIgnoreCase) != -1))));

		}

		public List<Album> GetResults(ISession session) {

			return session.Query<ArtistName>()
				.Where(an => an.Value.Contains(artistName))
				.SelectMany(an => an.Artist.AllAlbums)
				.Select(an => an.Album)
				.ToList();

		}

	}
}
