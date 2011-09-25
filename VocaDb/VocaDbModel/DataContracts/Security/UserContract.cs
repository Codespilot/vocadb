using System;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	public class UserContract {

		public UserContract() {}

		public UserContract(User user) {

			ParamIs.NotNull(() => user);

			CreateDate = user.CreateDate;
			Email = user.Email;
			Id = user.Id;
			Name = user.Name;
			PermissionFlags = user.PermissionFlags;

		}

		public DateTime CreateDate { get; set; }

		public string Email { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public PermissionFlags PermissionFlags { get; set; }

	}

}
