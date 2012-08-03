using System;

namespace VocaDb.Model.Domain.Tags {

	[Flags]
	public enum TagEditableFields {

		Nothing			= 0,

		CategoryName	= 1,

		Description		= 2,

	}

}
