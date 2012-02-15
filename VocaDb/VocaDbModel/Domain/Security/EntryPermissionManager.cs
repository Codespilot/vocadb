using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Domain.Security {

	public static class EntryPermissionManager {

		private static EntryStatus[] normalStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished };
		private static EntryStatus[] trustedStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished, EntryStatus.Approved };

		private static bool IsTrusted(IUserPermissionContext permissionContext) {
			return permissionContext.HasPermission(PermissionFlags.DeleteEntries);
		}

		public static EntryStatus[] AllowedEntryStatuses(IUserPermissionContext permissionContext) {

			if (!permissionContext.HasPermission(PermissionFlags.ManageDatabase))
				return new EntryStatus[] {};

			if (IsTrusted(permissionContext))
				return trustedStatusPermissions;

			return normalStatusPermissions;

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			if (!permissionContext.HasPermission(PermissionFlags.ManageDatabase))
				return false;

			if (IsTrusted(permissionContext))
				return (trustedStatusPermissions.Contains(entry.Status));

			return (normalStatusPermissions.Contains(entry.Status));

		}

		public static void VerifyEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			if (!CanEdit(permissionContext, entry))
				throw new NotAllowedException();				

		}

	}
}
