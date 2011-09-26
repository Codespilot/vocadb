using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class AlbumContract {

		public AlbumContract() { }

		public AlbumContract(Album album, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => album);

			Id = album.Id;
			Name = album.TranslatedName[languagePreference];
			ReleaseDate = album.ReleaseDate;

		}

		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime ReleaseDate { get; set; }

	}

}
