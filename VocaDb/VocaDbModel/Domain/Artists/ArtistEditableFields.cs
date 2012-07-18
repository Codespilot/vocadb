using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistEditableFields {

		Nothing			= 0,

		Albums			= 1,

		ArtistType		= 2,

		Description		= 4,

		Groups			= 8,

		Names			= 16,

		OriginalName	= 32,

		Picture			= 64,

		Pictures		= 128,

		Status			= 256,

		WebLinks		= 512

	}

}
