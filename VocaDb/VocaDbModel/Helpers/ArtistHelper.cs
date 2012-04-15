using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		/// <summary>
		/// Artist types able to work as groups/circles for an artist.
		/// </summary>
		public static readonly ArtistType[] ArtistGroupTypes = new[] {
			ArtistType.Circle, ArtistType.Label, ArtistType.OtherGroup
		};

		public static readonly Dictionary<ArtistType, ArtistCategories> CategoriesForTypes = new Dictionary<ArtistType, ArtistCategories> {
			{ ArtistType.Circle, ArtistCategories.Circle },
			{ ArtistType.Label, ArtistCategories.Label },
			{ ArtistType.OtherGroup, ArtistCategories.Circle },
			{ ArtistType.OtherVocalist, ArtistCategories.Vocalist },
			{ ArtistType.Producer, ArtistCategories.Producer },
			{ ArtistType.Unknown, ArtistCategories.Other },
			{ ArtistType.UTAU, ArtistCategories.Vocalist },
			{ ArtistType.Vocaloid, ArtistCategories.Vocalist },
		};

		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static readonly ArtistType[] CustomizableTypes = new[] {
			ArtistType.OtherGroup, ArtistType.OtherVocalist, ArtistType.Producer, 
			ArtistType.Unknown
		};

		public static readonly ArtistType[] ProducerTypes = new[] {
			ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup
		};

		/// <summary>
		/// Artists allowed for a song.
		/// </summary>
		public static readonly ArtistType[] SongArtistTypes = new[] {
			ArtistType.Unknown, ArtistType.OtherGroup, ArtistType.OtherVocalist,
			ArtistType.Producer, ArtistType.UTAU, ArtistType.Vocaloid
		};

		public static readonly ArtistType[] VocalistTypes = new[] {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVocalist
		};

		public static TranslatedStringWithDefault GetArtistString(IEnumerable<IArtistWithSupport> artists) {

			ParamIs.NotNull(() => artists);

			var matched = artists.Where(a => a.Artist.ArtistType != ArtistType.Label && !a.IsSupport).Select(a => a.Artist).ToArray();
			var producers = matched.Where(a => ProducerTypes.Contains(a.ArtistType));
			var performers = matched.Where(a => VocalistTypes.Contains(a.ArtistType));
			const string various = "Various artists";

			if (producers.Count() >= 4 || (!producers.Any() && performers.Count() >= 4))
				return new TranslatedStringWithDefault(various, various, various, various);

			var performerNames = performers.Select(m => m.TranslatedName);
			var producerNames =	producers.Select(m => m.TranslatedName);

			if (producers.Any() && performers.Any() && producers.Count() + performers.Count() >= 5) {

				return TranslatedStringWithDefault.Create(lang => string.Format("{0} feat. various",
					string.Join(", ", producerNames.Select(p => p[lang]))));

			} else if (producers.Any() && performers.Any()) {

				return TranslatedStringWithDefault.Create(lang => string.Format("{0} feat. {1}",
					string.Join(", ", producerNames.Select(p => p[lang])),
					string.Join(", ", performerNames.Select(p => p[lang]))));

			} else {

				return TranslatedStringWithDefault.Create(lang => string.Join(", ", matched.Select(a => a.TranslatedName[lang])));

			}

		}

		public static bool IsCustomizable(ArtistType at) {

			return CustomizableTypes.Contains(at);

		}

	}

}
