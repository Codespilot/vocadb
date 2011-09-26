using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	public class AlbumForEditContract : AlbumDetailsContract {

		public AlbumForEditContract() {}

		public AlbumForEditContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			Names = album.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();

		}

		public LocalizedStringWithIdContract[] Names { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

	}

}
