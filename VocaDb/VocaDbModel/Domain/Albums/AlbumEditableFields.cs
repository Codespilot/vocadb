using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Albums {

	[Flags]
	public enum AlbumEditableFields {

		Nothing			= 0,

		Cover			= 1,

		Description		= 2,

		DiscType		= 4,

		Names			= 8,

		OriginalRelease	= 16,

		WebLinks		= 32

	}

}
