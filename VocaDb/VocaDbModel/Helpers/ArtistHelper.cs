using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		public static readonly ArtistType[] ProducerTypes = new[] {
			ArtistType.Producer, ArtistType.Circle
		};

		public static readonly ArtistType[] VocalistTypes = new[] {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVocalist
		};

		public static string GetArtistString(IEnumerable<Artist> artists) {

			if (artists.Count() >= 4)
				return "Various artists";

			var producers = artists.Where(a => ProducerTypes.Contains(a.ArtistType)).Select(m => m.Name);
			var performers = artists.Where(a => VocalistTypes.Contains(a.ArtistType)).Select(m => m.Name);

			if (producers.Any() && performers.Any())
				return string.Format("{0} feat. {1}", string.Join(", ", producers), string.Join(", ", performers));
			else
				return string.Join(", ", artists.Select(m => m.Name));

		}


	}

}
