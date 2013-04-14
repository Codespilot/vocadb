using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Users {

	public class UserWithActivityEntriesContract : UserContract {

		public UserWithActivityEntriesContract(User user, IEnumerable<ActivityEntryContract> activityEntries, ContentLanguagePreference languagePreference)
			: base(user) {

			ActivityEntries = activityEntries.ToArray();

		}

		public ActivityEntryContract[] ActivityEntries { get; set; }

	}

}
