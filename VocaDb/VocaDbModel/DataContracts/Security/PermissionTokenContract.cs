using System;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {
	public class PermissionTokenContract {

		public PermissionTokenContract() { }

		public PermissionTokenContract(PermissionToken token) {

			Id = token.Id;
			Name = token.Name;

		}

		public Guid Id { get; set; }

		public string Name { get; set; }

	}

}
