using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class ArchivedAlbumVersionDetailsContract : ArchivedAlbumVersionContract {

		public ArchivedAlbumVersionDetailsContract() { }

		public ArchivedAlbumVersionDetailsContract(ArchivedAlbumVersion archived, ContentLanguagePreference languagePreference)
			: base(archived) {

			Album = new AlbumContract(archived.Album, languagePreference);
			Data = ArchivedAlbumContract.GetAllProperties(archived);

			Name = Album.Name;

		}

		public AlbumContract Album { get; set; }

		public ArchivedAlbumContract Data { get; set; }

		public string Name { get; set; }

	}

}
