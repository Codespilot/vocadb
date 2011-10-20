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
		/// User is allowed to edit the artist/album/song entries and create new entries
		/// </summary>
		ManageDatabase	= 2,

		MergeEntries	= 4,

		DeleteEntries	= 8,

		/// <summary>
		/// User is allowed to manage user accounts
		/// </summary>
		ManageUsers		= 16,

		Admin			= 32,

		MikuDbImport	= 64,

		Default			= (EditProfile | ManageDatabase)

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
