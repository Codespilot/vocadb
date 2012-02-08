using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Artists {

	public enum ArtistRole {

		Unspecified		= 0,

		Composer		= 1,

		Arranger		= 2,

		Lyricist		= 4,

		Performer		= 8,

		Illustrator		= 16,

		Other			= 32

	}
}
