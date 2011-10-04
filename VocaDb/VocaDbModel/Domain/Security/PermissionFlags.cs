using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum PermissionFlags {

		Nothing			= 0,

		/// <summary>
		/// User is allowed to edit his own profile
		/// </summary>
		EditProfile		= 1,

		/// <summary>
		/// User is allowed to edit the artist/album/song entries
		/// </summary>
		ManageDatabase	= 2,

		/// <summary>
		/// User is allowed to manage user accounts
		/// </summary>
		ManageUsers		= 4

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
