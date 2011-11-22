using System;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongEditableFields {

		Nothing			= 0,

		Artists			= 1,

		Lyrics			= 2,

		Names			= 4,

		Notes			= 8,

		OriginalVersion	= 16,

		PVs				= 32,

		SongType		= 64,

		WebLinks		= 128

	}

}
