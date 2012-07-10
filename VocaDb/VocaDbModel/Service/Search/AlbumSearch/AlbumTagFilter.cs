using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumTagFilter : ISearchFilter<Album> {

		private readonly string tagName;

		public AlbumTagFilter(string tagName) {
			this.tagName = tagName;
		}

		public QueryCost Cost {
			get { return QueryCost.Medium; }
		}

		public void FilterResults(List<Album> albums, ISession session) {

			albums.RemoveAll(a => !(a.Tags.HasTag(tagName)));

		}

		public List<Album> GetResults(ISession session) {

			return session.Query<AlbumTagUsage>()
				.Where(a => a.Tag.Name == tagName)
				.Select(a => a.Album)
				.ToList();

		}

	}

}
