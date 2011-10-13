using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class AlbumForEditContract : AlbumDetailsContract {

		public AlbumForEditContract() {}

		public AlbumForEditContract(Album album, IEnumerable<Artist> allLabels , ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			AllLabels = allLabels.Select(l => new ArtistContract(l, languagePreference)).ToArray();
			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			TranslatedName = new TranslatedStringContract(album.TranslatedName);

		}

		public ArtistContract[] AllLabels { get; set; }

		public LocalizedStringWithIdContract[] Names { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

	}

}
