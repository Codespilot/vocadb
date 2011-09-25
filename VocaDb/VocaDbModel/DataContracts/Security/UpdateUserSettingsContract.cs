namespace VocaDb.Model.DataContracts.Security {

	public class UpdateUserSettingsContract : UserContract {

		public string NewPass { get; set; }

		public string OldPass { get; set; }

	}

}
