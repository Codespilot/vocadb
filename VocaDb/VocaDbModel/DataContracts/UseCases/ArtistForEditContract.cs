using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class ArtistForEditContract : ArtistDetailsContract {

		public ArtistForEditContract(Artist artist, ContentLanguagePreference languagePreference, IEnumerable<Artist> allCircles)
			: base(artist, languagePreference) {

			ParamIs.NotNull(() => allCircles);

			AllCircles = allCircles.OrderBy(a => a.TranslatedName[languagePreference]).Select(a => new ArtistContract(a, languagePreference)).ToArray();
			Names = artist.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();

		}

		public ArtistContract[] AllCircles { get; private set; }

		public LocalizedStringWithIdContract[] Names { get; private set; }

	}

}
