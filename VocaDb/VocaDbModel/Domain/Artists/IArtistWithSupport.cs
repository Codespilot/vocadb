﻿namespace VocaDb.Model.Domain.Artists {

	public interface IArtistWithSupport {

		Artist Artist { get; }

		bool IsSupport { get; }

	}
}
