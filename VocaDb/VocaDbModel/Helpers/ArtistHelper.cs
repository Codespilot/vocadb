using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		public static string GetArtistString(IEnumerable<Artist> artists) {

			var producers = artists.Where(a => a.ArtistType == ArtistType.Producer || a.ArtistType == ArtistType.Circle).Select(m => m.Name);
			var performers = artists.Where(a => a.ArtistType == ArtistType.Performer).Select(m => m.Name);

			if (producers.Any() && performers.Any())
				return string.Format("{0} feat. {1}", string.Join(", ", producers), string.Join(", ", performers));
			else
				return string.Join(", ", artists.Select(m => m.Name));

		}


	}

}
