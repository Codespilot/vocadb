using VocaDb.Model;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;

namespace VocaDb.Web.Models.MikuDbAlbums {

	public class Index {

		public Index() {}

		public Index(MikuDbAlbumContract[] albums, AlbumStatus status) {

			ParamIs.NotNull(() => albums);

			Albums = albums;
			AllStatuses = EnumVal<AlbumStatus>.Values;
			Status = status;

		}

		public MikuDbAlbumContract[] Albums { get; set; }

		public AlbumStatus[] AllStatuses { get; set; }

		public AlbumStatus Status { get; set; }

	}

}