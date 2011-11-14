using System;

namespace VocaDb.Model.Domain.Albums {

	[Flags]
	public enum AlbumEditableFields {

		Nothing			= 0,

		Artists			= 1,

		Cover			= 2,

		Description		= 4,

		DiscType		= 8,

		Names			= 16,

		OriginalRelease	= 32,

		Tracks			= 64,

		WebLinks		= 128

	}

}
