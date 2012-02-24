using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserContract : UserBaseContract {

		public UserContract() {}

		public UserContract(User user)
			: base(user) {

			ParamIs.NotNull(() => user);

			Active = user.Active;
			AdditionalPermissions = user.AdditionalPermissions;
			AnonymousActivity = user.AnonymousActivity;
			CreateDate = user.CreateDate;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			EffectivePermissions = user.EffectivePermissions;
			EmailOptions = user.EmailOptions;
			GroupId = user.GroupId;
			PreferredVideoService = user.PreferredVideoService;

		}

		[DataMember]
		public bool Active { get; set; }

		[DataMember]
		public PermissionCollection AdditionalPermissions { get; set; }

		[DataMember]
		public bool AnonymousActivity { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[DataMember]
		public PermissionCollection EffectivePermissions { get; set; }

		[DataMember]
		public UserEmailOptions EmailOptions { get; set; }

		[DataMember]
		public UserGroupId GroupId { get; set; }

		[DataMember]
		public bool HasUnreadMessages { get; set; }

		[DataMember]
		public PVService PreferredVideoService { get; set; }

	}

}
