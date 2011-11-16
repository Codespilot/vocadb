using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongEditableFields {

		Nothing		= 0,

		Artists		= 1,

		Lyrics		= 2,

		Names		= 4,

		Notes		= 8,

		PVs			= 16,

		SongType	= 32,

		WebLinks	= 64

	}

}
