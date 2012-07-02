using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search {

	public class AlbumArtistFilter {

		private readonly string artistName;

		public AlbumArtistFilter(string artistName) {
			this.artistName = artistName;
		}

		public void FilterResults(List<Album> albums, ISession session) {

			albums.RemoveAll(a => !(
				a.ArtistString.Default.IndexOf(artistName, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Japanese.IndexOf(artistName, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Romaji.IndexOf(artistName, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.English.IndexOf(artistName, StringComparison.InvariantCultureIgnoreCase) != -1));

		}

		public List<Album> GetResults(ISession session) {

			return session.Query<Album>()
				.Where(a => 
					a.ArtistString.Default.Contains(artistName)
					&& a.ArtistString.Japanese.Contains(artistName)
					&& a.ArtistString.Romaji.Contains(artistName)
					&& a.ArtistString.English.Contains(artistName))
				.ToList();

		}

	}
}
