using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		public static readonly ArtistType[] ArtistTypes = new[] {
			ArtistType.Producer
		};

		public static readonly ArtistType[] LabelTypes = new[] {
			ArtistType.Circle, ArtistType.Label, ArtistType.OtherGroup
		};

		public static readonly ArtistType[] ProducerTypes = new[] {
			ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup
		};

		public static readonly ArtistType[] VocalistTypes = new[] {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVocalist
		};

		public static TranslatedString GetArtistString(IEnumerable<IArtistWithSupport> artists) {

			ParamIs.NotNull(() => artists);

			var matched = artists.Where(a => a.Artist.ArtistType != ArtistType.Label && !a.IsSupport).Select(a => a.Artist).ToArray();
			var producers = matched.Where(a => ProducerTypes.Contains(a.ArtistType));
			var performers = matched.Where(a => VocalistTypes.Contains(a.ArtistType));
			const string various = "Various artists";

			if (producers.Count() >= 4 || (!producers.Any() && performers.Count() >= 4))
				return new TranslatedString(various, various, various);

			var performerNames = performers.Select(m => m.TranslatedName);
			var producerNames =	producers.Select(m => m.TranslatedName);

			if (producers.Any() && performers.Any() && producers.Count() + performers.Count() >= 5) {

				return TranslatedString.Create(lang => string.Format("{0} feat. various",
					string.Join(", ", producerNames.Select(p => p[lang]))));

			} else if (producers.Any() && performers.Any()) {

				return TranslatedString.Create(lang => string.Format("{0} feat. {1}",
					string.Join(", ", producerNames.Select(p => p[lang])),
					string.Join(", ", performerNames.Select(p => p[lang]))));

			} else {

				return TranslatedString.Create(lang => string.Join(", ", matched.Select(a => a.TranslatedName[lang])));

			}

		}

	}

}
