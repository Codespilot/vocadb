using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImportResult {

		public MikuDbAlbumContract AlbumContract { get; set; }

		public string Message { get; set; }

	}

}
