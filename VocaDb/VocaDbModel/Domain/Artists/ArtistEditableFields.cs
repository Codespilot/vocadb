using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistEditableFields {

		Nothing		= 0,

		ArtistType	= 1,

		Description	= 2,

		Groups		= 4,

		Names		= 8,

		Picture		= 16,

		WebLinks	= 32

	}

}
