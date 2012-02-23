using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace VocaDb.Model.Domain.Security {

	public class PermissionCollection : IEnumerable<PermissionFlags> {

		private readonly ISet<PermissionFlags> permissions = new HashSet<PermissionFlags>();

		private void AddAll(IEnumerable<PermissionFlags> flags) {

			foreach (var flag in flags)
				permissions.Add(flag);

		}

		private void AddAll(PermissionFlags flags) {

			AddAll(EnumVal<PermissionFlags>.Values.Where(f => flags.IsSet(f)));

		}

		public PermissionCollection() { }

		public PermissionCollection(IEnumerable<PermissionFlags> permissions)
			: this() {
			AddAll(permissions);
		}

		public PermissionCollection(PermissionFlags permissionBitArray)
			: this() {
			AddAll(permissionBitArray);
		}

		public IEnumerable<PermissionFlags> Permissions {
			get { return permissions; }
		}

		public PermissionFlags PermissionBitArray {
			get {
				return Permissions.Aggregate((list, item) => list |= item);
			}
			set {
				permissions.Clear();
				AddAll(value);
			}
		}

		public IEnumerator<PermissionFlags> GetEnumerator() {
			return Permissions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool Has(PermissionFlags flag) {

			return (flag == PermissionFlags.Nothing || permissions.Contains(flag));

		}

		public PermissionCollection Merge(PermissionCollection collection) {

			return new PermissionCollection(Permissions.Concat(collection.Permissions));

		}

	}
}
