using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UpdateUserSettingsContract : UserContract {

		public string AboutMe { get; set; }

		public string Location { get; set; }

		public string NewPass { get; set; }

		public string OldPass { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
