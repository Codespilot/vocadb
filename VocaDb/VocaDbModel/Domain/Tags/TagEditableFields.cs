using System;

namespace VocaDb.Model.Domain.Tags {

	[Flags]
	public enum TagEditableFields {

		Nothing			= 0,

		AliasedTo		= 1,

		CategoryName	= 2,

		Description		= 4,

		Picture			= 8,

		Status			= 16

	}

}
