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

		Pictures		= 128,

		PVs				= 256,

		Status			= 512,

		Tracks			= 1024,

		WebLinks		= 2048

	}

}
