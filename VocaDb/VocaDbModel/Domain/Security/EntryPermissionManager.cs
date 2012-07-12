﻿using System;
using System.Linq;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Security {

	public static class EntryPermissionManager {

		private static readonly EntryStatus[] allPermissions = EnumVal<EntryStatus>.Values;
		private static readonly EntryStatus[] normalStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished };
		private static readonly EntryStatus[] trustedStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished, EntryStatus.Approved };

		public static EntryStatus[] AllowedEntryStatuses(IUserPermissionContext permissionContext) {

			if (permissionContext.HasPermission(PermissionToken.LockEntries))
				return allPermissions;

			if (permissionContext.HasPermission(PermissionToken.ApproveEntries))
				return trustedStatusPermissions;

			if (permissionContext.HasPermission(PermissionToken.ManageDatabase))
				return normalStatusPermissions;

			return new EntryStatus[] {};

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, SongList songList) {

			if (songList.FeaturedList && CanManageFeaturedLists(permissionContext))
				return true;

			return (songList.Author.IsTheSameUser(permissionContext.LoggedUser));

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			if (!permissionContext.HasPermission(PermissionToken.ManageDatabase))
				return false;

			if (permissionContext.HasPermission(PermissionToken.LockEntries))
				return true;

			if (permissionContext.HasPermission(PermissionToken.ApproveEntries))
				return (trustedStatusPermissions.Contains(entry.Status));

			return (normalStatusPermissions.Contains(entry.Status));

		}

		public static bool CanManageFeaturedLists(IUserPermissionContext permissionContext) {

			return permissionContext.HasPermission(PermissionToken.EditFeaturedLists);

		}

		public static void VerifyAccess<T>(IUserPermissionContext permissionContext, T entry, Func<IUserPermissionContext, T, bool> accessCheck) where T : class, IEntryBase {

			ParamIs.NotNull(() => entry);

			if (!accessCheck(permissionContext, entry))
				throw new NotAllowedException();

		}

		public static void VerifyEdit(IUserPermissionContext permissionContext, SongList entry) {

			VerifyAccess(permissionContext, entry, CanEdit);

		}

		public static void VerifyEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			VerifyAccess(permissionContext, entry, CanEdit);

		}

	}
}
