using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class ArchivedAlbumVersionDetailsContract : ArchivedAlbumVersionContract {

		public ArchivedAlbumVersionDetailsContract() { }

		public ArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ContentLanguagePreference languagePreference)
			: base(archived) {

			Album = (archived.Album != null ? new AlbumContract(archived.Album, languagePreference) : null);
			Data = XmlHelper.DeserializeFromXml<ArchivedAlbumContract>(archived.Data);

			if (Album != null) {
				Name = Album.Name;
			} else if (Data.TranslatedName != null) {

				var translatedName = new TranslatedString(Data.TranslatedName);
				Name = translatedName[languagePreference];

			}

		}

		public AlbumContract Album { get; set; }

		public ArchivedAlbumContract Data { get; set; }

		public string Name { get; set; }

	}

}
