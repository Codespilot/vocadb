using System;

namespace VocaDb.Model.Domain.Songs {

	[Flags]
	public enum SongEditableFields {

		Nothing			= 0,

		Artists			= 1,

		Lyrics			= 2,

		Names			= 4,

		Notes			= 8,

		OriginalName	= 16,

		OriginalVersion	= 32,

		PVs				= 64,

		SongType		= 128,

		WebLinks		= 128

	}

}
