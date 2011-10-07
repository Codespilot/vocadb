using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumContract {

		public AlbumContract() { }

		public AlbumContract(Album album, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => album);

			ArtistString = album.ArtistString;
			DiscType = album.DiscType;
			Id = album.Id;
			Name = album.TranslatedName[languagePreference];
			ReleaseDate = album.ReleaseDate;

		}

		public string ArtistString { get; set; }

		public DiscType DiscType { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public string ProductCode { get; set; }

		public DateTime? ReleaseDate { get; set; }

	}

}
