using System;

namespace VocaDb.Model.Domain.Albums {

	[Flags]
	public enum AlbumEditableFields {

		Nothing			= 0,

		Artists			= 1,

		Barcode			= 2,

		Cover			= 4,

		Description		= 8,

		DiscType		= 16,

		Names			= 32,

		OriginalName	= 64,

		OriginalRelease	= 128,

		Pictures		= 256,

		PVs				= 512,

		Status			= 1024,

		Tracks			= 2048,

		WebLinks		= 4096

	}

}
