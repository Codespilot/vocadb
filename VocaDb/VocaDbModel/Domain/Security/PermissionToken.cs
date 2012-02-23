using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Security {

	public struct PermissionToken : IEquatable<PermissionToken> {

		private readonly Guid id;
		private readonly string name;

		private static PermissionToken New(string guid, string name) {
			return new PermissionToken(new Guid(guid), name);
		}

		public static readonly PermissionToken EditProfile = New("4f79b01a-7154-4a7f-bc87-a8a9259a9905", "EditProfile");
		public static readonly PermissionToken ManageDatabase = New("d762d720-79ef-4e60-8397-1d638c26d82b", "ManageDatabase");

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
