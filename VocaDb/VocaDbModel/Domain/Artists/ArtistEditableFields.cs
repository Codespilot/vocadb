﻿using System;

namespace VocaDb.Model.Domain.Artists {

	[Flags]
	public enum ArtistEditableFields {

		Nothing			= 0,

		Albums			= 1,

		ArtistType		= 2,

		BaseVoicebank	= 4,

		Description		= 8,

		Groups			= 16,

		Names			= 32,

		OriginalName	= 64,

		Picture			= 128,

		Pictures		= 256,

		Status			= 512,

		WebLinks		= 1024

	}

}
