using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserContract {

		public UserContract() {}

		public UserContract(User user) {

			ParamIs.NotNull(() => user);

			Active = user.Active;
			AdditionalPermissions = user.AdditionalPermissions;
			CreateDate = user.CreateDate;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			EffectivePermissions = user.EffectivePermissions;
			Email = user.Email;
			EmailOptions = user.EmailOptions;
			GroupId = user.GroupId;
			Id = user.Id;
			Name = user.Name;
			PreferredVideoService = user.PreferredVideoService;

		}

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public PermissionFlags AdditionalPermissions { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[DataMember]
		public PermissionFlags EffectivePermissions { get; set; }

		[DataMember]
		public string Email { get; set; }

		[DataMember]
		public UserEmailOptions EmailOptions { get; set; }

		[DataMember]
		public UserGroupId GroupId { get; set; }

		[DataMember]
		public bool HasUnreadMessages { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public PVService PreferredVideoService { get; set; }

	}

}
