using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum PermissionFlags {

		Nothing			= 0,

		CreatePolls		= 1,

		ManageDatabase	= 2

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
