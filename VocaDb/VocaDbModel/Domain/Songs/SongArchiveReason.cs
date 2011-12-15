using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs {

	public enum SongArchiveReason {

		Unknown,

		Created,

		Merged,

		AutoImportedFromMikuDb,

		PropertiesUpdated,

		Reverted

	}

}
