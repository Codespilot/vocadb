using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Web.Models.MikuDbAlbums {

	public class PrepareAlbumsForImport {

		public PrepareAlbumsForImport() {}

		public PrepareAlbumsForImport(InspectedAlbum[] albums) {
			Albums = albums;
		}

		public InspectedAlbum[] Albums { get; set; }

	}
}