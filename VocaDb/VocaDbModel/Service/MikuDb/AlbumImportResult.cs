using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImportResult {

		public MikuDbAlbumContract AlbumContract { get; set; }

		/// <summary>
		/// Result message, such as a warning explaining why the album couldn't be imported.
		/// </summary>
		public string Message { get; set; }

	}

}
