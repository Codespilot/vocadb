using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserContract {

		public UserContract() {}

		public UserContract(User user) {

			ParamIs.NotNull(() => user);

			CreateDate = user.CreateDate;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			Id = user.Id;
			Name = user.Name;
			PermissionFlags = user.PermissionFlags;

		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[DataMember]
		public string Email { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public PermissionFlags PermissionFlags { get; set; }

	}

}
