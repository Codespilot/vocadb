using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Helpers {

	public static class Login {

		public static bool CanAccessManageMenu {
			get {

				return Manager.HasPermission(PermissionFlags.Admin)
					|| Manager.HasPermission(PermissionFlags.ManageUserBlocks)
					|| Manager.HasPermission(PermissionFlags.ManageUsers)
					|| Manager.HasPermission(PermissionFlags.ViewAuditLog);

			}
		}

		public static bool CanDeleteEntries {
			get {
				return Manager.HasPermission(PermissionFlags.DeleteEntries);
			}
		}

		public static bool CanManageDb {
			get {
				return Manager.HasPermission(PermissionFlags.ManageDatabase);
			}
		}

		public static bool CanManageUsers {
			get {
				return Manager.HasPermission(PermissionFlags.ManageUsers);
			}
		}

		public static bool CanMergeEntries {
			get {
				return Manager.HasPermission(PermissionFlags.MergeEntries);
			}
		}

		public static LoginManager Manager {
			get {
				return MvcApplication.LoginManager;
			}
		}

		public static UserContract User {
			get {
				return Manager.LoggedUser;
			}
		}

		public static bool CanDeleteComment(CommentContract comment) {

			ParamIs.NotNull(() => comment);

			return Manager.HasPermission(PermissionFlags.ManageUserBlocks)
				|| (comment.Author != null && User != null && comment.Author.Id == User.Id);

		}

	}

}