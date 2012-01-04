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

		OriginalName	= 32,

		OriginalRelease	= 64,

		PVs				= 128,

		Status			= 256,

		Tracks			= 512,

		WebLinks		= 1024

	}

}
