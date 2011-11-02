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
			ArtistType.Circle, ArtistType.Label
		};

		public static readonly ArtistType[] ProducerTypes = new[] {
			ArtistType.Producer, ArtistType.Circle
		};

		public static readonly ArtistType[] VocalistTypes = new[] {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVocalist
		};

		public static TranslatedString GetArtistString(IEnumerable<Artist> artists) {

			if (artists.Count() >= 4)
				return new TranslatedString("Various artists", "Various artists", "Various artists");

			var producers = artists.Where(a => ProducerTypes.Contains(a.ArtistType)).Select(m => m.TranslatedName);
			var performers = artists.Where(a => VocalistTypes.Contains(a.ArtistType)).Select(m => m.TranslatedName);

			if (producers.Any() && performers.Any()) {

				return TranslatedString.Create(lang => string.Format("{0} feat. {1}", 
					string.Join(", ", producers.Select(p => p[lang])), 
					string.Join(", ", performers.Select(p => p[lang]))));

				//return string.Format("{0} feat. {1}", string.Join(", ", producers), string.Join(", ", performers));
			} else {

				return TranslatedString.Create(lang => string.Join(", ", artists.Select(a => a.TranslatedName[lang])));

				//return string.Join(", ", artists.Select(m => m.Name));
			}

		}


	}

}
