using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class AlbumForEditContract : AlbumDetailsContract {

		public AlbumForEditContract() {}

		public AlbumForEditContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			TranslatedName = new TranslatedStringContract(album.TranslatedName);

		}

		public LocalizedStringWithIdContract[] Names { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

	}

}
