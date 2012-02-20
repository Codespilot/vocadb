using System;
using System.Linq;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Security {

	public static class EntryPermissionManager {

		private static readonly EntryStatus[] allPermissions = EnumVal<EntryStatus>.Values;
		private static readonly EntryStatus[] normalStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished };
		private static readonly EntryStatus[] trustedStatusPermissions = new[] { EntryStatus.Draft, EntryStatus.Finished, EntryStatus.Approved };

		private static bool IsMod(IUserPermissionContext permissionContext) {
			return permissionContext.HasPermission(PermissionFlags.ManageUserBlocks);
		}

		private static bool IsTrusted(IUserPermissionContext permissionContext) {
			return permissionContext.HasPermission(PermissionFlags.DeleteEntries);
		}

		public static EntryStatus[] AllowedEntryStatuses(IUserPermissionContext permissionContext) {

			if (!permissionContext.HasPermission(PermissionFlags.ManageDatabase))
				return new EntryStatus[] {};

			if (IsMod(permissionContext))
				return allPermissions;

			if (IsTrusted(permissionContext))
				return trustedStatusPermissions;

			return normalStatusPermissions;

		}

		public static bool CanCreateFeaturedLists(IUserPermissionContext permissionContext) {

			return IsTrusted(permissionContext);

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, SongList songList) {

			if (IsMod(permissionContext))
				return true;

			if (songList.FeaturedCategory != SongListFeaturedCategory.Nothing && IsTrusted(permissionContext))
				return true;

			return (permissionContext.Equals(songList.Author));

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			if (!permissionContext.HasPermission(PermissionFlags.ManageDatabase))
				return false;

			if (IsMod(permissionContext))
				return true;

			if (IsTrusted(permissionContext))
				return (trustedStatusPermissions.Contains(entry.Status));

			return (normalStatusPermissions.Contains(entry.Status));

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
