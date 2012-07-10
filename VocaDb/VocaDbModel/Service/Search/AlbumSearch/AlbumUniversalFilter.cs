using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumUniversalFilter : ISearchFilter<Album> {

		private readonly string term;

		public AlbumUniversalFilter(string term) {
			this.term = term;
		}

		public QueryCost Cost {
			get { return QueryCost.VeryHigh; }
		}

		public void FilterResults(List<Album> albums, ISession session) {

			albums.RemoveAll(a => !(
				a.Names.Any(n => n.Value.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1)
				|| (a.ArtistString.Default.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Japanese.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Romaji.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.English.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1)));

		}

		public List<Album> GetResults(ISession session) {

			return session.Query<AlbumName>()
				.Where(n => n.Value.Contains(term) ||
					n.Album.ArtistString.Default.Contains(term)
					&& n.Album.ArtistString.Japanese.Contains(term)
					&& n.Album.ArtistString.Romaji.Contains(term)
					&& n.Album.ArtistString.English.Contains(term))
				.Select(a => a.Album)
				.Distinct()
				.ToList();

			/*return session.Query<AlbumName>()
				.Where(n => n.Value.Contains(term))
				.Select(a => a.Album)
				.Where(a => 
					a.ArtistString.Default.Contains(term)
					&& a.ArtistString.Japanese.Contains(term)
					&& a.ArtistString.Romaji.Contains(term)
					&& a.ArtistString.English.Contains(term))
				.ToList();*/

		}

	}

}
