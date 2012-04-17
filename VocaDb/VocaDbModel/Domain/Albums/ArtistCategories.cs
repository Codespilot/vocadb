using System;

namespace VocaDb.Model.Domain.Albums {

	[Flags]
	public enum ArtistCategories {

		Nothing		= 0,

		Vocalist	= 1,

		Producer	= 2,

		Label		= 4,

		Circle		= 8,

		Other		= 16

	}

}
