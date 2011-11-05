using System;

namespace VocaDb.Model.Domain.PVs {

	public enum PVService {

		NicoNicoDouga	= 1,

		Youtube			= 2,

	}

	[Flags]
	public enum PVServices {

		Nothing			= 0,

		NicoNicoDouga	= PVService.NicoNicoDouga,

		Youtube			= PVService.Youtube,

	}

}
