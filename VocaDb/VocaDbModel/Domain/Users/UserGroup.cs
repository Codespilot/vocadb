using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	public class UserGroup {

		private static readonly UserGroup limited = new UserGroup(UserGroupId.Limited, PermissionToken.EditProfile);

		private static readonly UserGroup regular = new UserGroup(UserGroupId.Regular,
			limited, PermissionToken.CreateComments, PermissionToken.ManageDatabase);

		private static readonly UserGroup trusted = new UserGroup(UserGroupId.Trusted,
			regular, PermissionToken.ApproveEntries, PermissionToken.DeleteEntries, PermissionToken.EditFeaturedLists, PermissionToken.MergeEntries);

		private static readonly UserGroup mod = new UserGroup(UserGroupId.Moderator,
			trusted, PermissionToken.AccessManageMenu, PermissionToken.DeleteComments, PermissionToken.DesignatedStaff, PermissionToken.DisableUsers, 
			PermissionToken.EditNews,
			PermissionToken.LockEntries, PermissionToken.ReadRecentComments, PermissionToken.RestoreRevisions, PermissionToken.ViewAuditLog);

		private static readonly UserGroup admin = new UserGroup(UserGroupId.Admin,
			mod, PermissionToken.Admin, PermissionToken.ManageUserPermissions, PermissionToken.MikuDbImport);

		private static readonly Dictionary<UserGroupId, UserGroup> groups = new[] {
			limited, regular, trusted, mod, admin
		}.ToDictionary(g => g.Id);

		public static PermissionCollection GetPermissions(UserGroupId groupId) {

			if (!groups.ContainsKey(groupId))
				return new PermissionCollection();

			return groups[groupId].Permissions;

		}

		public static UserGroupId[] GroupIds {
			get { 
				return EnumVal<UserGroupId>.Values; 
			}
		}

		public UserGroup(UserGroupId id, UserGroup parent, params PermissionToken[] permissions) {
			this.Id = id;
			this.Permissions = parent.Permissions + new PermissionCollection(permissions);
		}

		public UserGroup(UserGroupId id, params PermissionToken[] permissions) {
			this.Id = id;
			this.Permissions = new PermissionCollection(permissions);
		}

		public UserGroup(UserGroupId id, PermissionCollection permissions) {
			this.Id = id;
			this.Permissions = permissions;
		}

		public UserGroupId Id { get; private set; }

		public PermissionCollection Permissions { get; private set; }

	}

	public enum UserGroupId {

		Nothing,

		Limited,

		Regular,

		Trusted,

		Moderator,

		Admin,

	}

}
