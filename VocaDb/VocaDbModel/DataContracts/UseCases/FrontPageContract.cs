using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.UseCases {

	public class FrontPageContract {

		public FrontPageContract(IEnumerable<ActivityEntry> activityEntries, IEnumerable<NewsEntry> newsEntries,
			ContentLanguagePreference languagePreference) {

			ActivityEntries = activityEntries.Select(e => new ActivityEntryContract(e, languagePreference)).ToArray();
			NewsEntries = newsEntries.Select(e => new NewsEntryContract(e)).ToArray();

		}

		public ActivityEntryContract[] ActivityEntries { get; set; }

		public NewsEntryContract[] NewsEntries { get; set; }

	}
}
