using System;
using System.Collections.Generic;

namespace VocaDb.Model.Domain.Security {

	public struct PermissionToken : IEquatable<PermissionToken> {

		private readonly Guid id;
		private readonly string name;

		private static PermissionToken New(string guid, string name) {
			return new PermissionToken(new Guid(guid), name);
		}

		public static readonly PermissionToken AccessAdmin =		New("1c98077f-f36f-4ef2-8cf3-cd9e347d389a", "AccessAdmin");
		public static readonly PermissionToken DeleteComments =		New("1b1dfcfa-6b96-4a8a-8aca-d76465439ffb", "DeleteComments");
		public static readonly PermissionToken DeleteEntries =		New("cc51c6b6-be93-4942-a6e4-fdf88f4520b9", "DeleteEntries");
		public static readonly PermissionToken DisableUsers =		New("cb46dfbe-5221-4af4-9968-53aec5faa3d4", "DisableUsers");
		public static readonly PermissionToken EditProfile =		New("4f79b01a-7154-4a7f-bc87-a8a9259a9905", "EditProfile");
		public static readonly PermissionToken ManageDatabase =		New("d762d720-79ef-4e60-8397-1d638c26d82b", "ManageDatabase");
		public static readonly PermissionToken MergeEntries =		New("eb336a5b-8455-4048-bc3a-8003dc522dc5", "MergeEntries");
		public static readonly PermissionToken MikuDbImport =		New("0b879c57-5eba-462a-b842-d9f7dd0befd8", "MikuDbImport");
		public static readonly PermissionToken RestoreRevisions =	New("e99a1e1c-1742-48c1-877b-17cb2964e8bc", "RestoreRevisions");
		public static readonly PermissionToken ViewAuditLog =		New("8d3d5395-12c9-440a-8120-4911034b9a7e", "ViewAuditLog");

		public static readonly PermissionToken[] All = { 
			AccessAdmin, DeleteComments, DeleteEntries, DisableUsers, EditProfile, 
			ManageDatabase, MergeEntries, MikuDbImport, RestoreRevisions, ViewAuditLog
		};

		public PermissionToken(Guid id, string name) {
			this.id = id;
			this.name = name;
		}

		public Guid Id {
			get { return id; }
		}

		public string Name {
			get { return name; }
		}

		public bool Equals(PermissionToken token) {
			return (token.Id == Id);
		}

		public override bool Equals(object obj) {

			if (!(obj is PermissionToken))
				return false;

			return Equals((PermissionToken)obj);

		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

	}

}
