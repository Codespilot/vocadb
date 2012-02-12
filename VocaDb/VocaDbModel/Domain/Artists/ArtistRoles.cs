using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistRoles {

		Unspecified		= 0,

		Composer		= 1,

		Arranger		= 2,

		Lyricist		= 4,

		Performer		= 8,

		Illustrator		= 16,

		Other			= 32

	}
}
