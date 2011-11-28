using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users {

	public class UserGroup {

		private static readonly UserGroup limited = new UserGroup(UserGroupId.Limited, PermissionFlags.EditProfile);

		private static readonly UserGroup regular = new UserGroup(UserGroupId.Regular, 
			limited.Permissions | PermissionFlags.ManageDatabase);

		private static readonly UserGroup trusted = new UserGroup(UserGroupId.Trusted, 
			regular.Permissions | PermissionFlags.DeleteEntries | PermissionFlags.MergeEntries | PermissionFlags.RestoreEntries);

		private static readonly UserGroup mod = new UserGroup(UserGroupId.Moderator, 
			trusted.Permissions | PermissionFlags.ManageUserBlocks | PermissionFlags.ViewAuditLog);

		private static readonly UserGroup admin = new UserGroup(UserGroupId.Admin, 
			mod.Permissions | PermissionFlags.Admin | PermissionFlags.ManageUsers | PermissionFlags.MikuDbImport);

		private static readonly Dictionary<UserGroupId, UserGroup> groups = new[] {
			limited, regular, trusted, mod, admin
		}.ToDictionary(g => g.Id);

		public static PermissionFlags GetPermissions(UserGroupId groupId) {

			if (!groups.ContainsKey(groupId))
				return PermissionFlags.Nothing;

			return groups[groupId].Permissions;

		}

		public static UserGroupId[] GroupIds {
			get { 
				return EnumVal<UserGroupId>.Values; 
			}
		}

		public UserGroup(UserGroupId id, PermissionFlags permissions) {
			this.Id = id;
			this.Permissions = permissions;
		}

		public UserGroupId Id { get; private set; }

		public PermissionFlags Permissions { get; private set; }

	}

	public enum UserGroupId {

		Limited,

		Regular,

		Trusted,

		Moderator,

		Admin,

	}

}
