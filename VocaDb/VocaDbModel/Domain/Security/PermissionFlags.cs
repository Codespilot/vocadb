using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum PermissionFlags {

		Nothing			= 0,

		ManageAlbums	= 1,

		ManageArtists	= 2,

		ManageSongs		= 4,

		ManageUsers		= 8

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
