using System;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	public class UserContract {

		public UserContract(User user) {

			ParamIs.NotNull(() => user);

			CreateDate = user.CreateDate;
			Id = user.Id;
			Name = user.Name;
			PermissionFlags = user.PermissionFlags;

		}

		public DateTime CreateDate { get; private set; }

		public int Id { get; private set; }

		public string Name { get; private set; }

		public PermissionFlags PermissionFlags { get; private set; }

	}

}
