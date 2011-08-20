using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum PermissionFlags {

		Nothing			= 0,

		CreateArtist	= 1,

		EditArtist		= 2,

		DeleteArtist	= 4,

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
