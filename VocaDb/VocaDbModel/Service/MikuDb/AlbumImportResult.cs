using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImportResult {

		public MikuDbAlbumContract AlbumContract { get; set; }

		public string Message { get; set; }

	}

}
