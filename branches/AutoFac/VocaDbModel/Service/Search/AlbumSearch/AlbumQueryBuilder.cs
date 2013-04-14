using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumQueryBuilder {

		public QueryPlan<Album> BuildPlan(string query) {

			var words = SearchParser.ParseQuery(query);
			var filters = new List<ISearchFilter<Album>>();

			var names = words.TakeAll(string.Empty);

			if (names.Any()) {
				filters.Add(new AlbumNameFilter(names.Select(n => n.Value)));
			}

			while (words.Any()) {

				var word = words.TakeNext();
				ISearchFilter<Album> filter;

				switch (word.PropertyName.ToLowerInvariant()) {
					case "artist":
						int artistId;
						if (int.TryParse(word.Value, out artistId))
							filter = new AlbumArtistFilter(artistId);
						else
							filter = new AlbumArtistNameFilter(word.Value);
						break;

					case "tag":
						filter = new AlbumTagFilter(word.Value);
						break;

					default:
						filter = new AlbumNameFilter(new[] { word.Value });
						break;

				}

				filters.Add(filter);

			}

			return new QueryPlan<Album>(filters);

		}

	}

}
