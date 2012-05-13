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
			IEnumerable<Album> newAlbums, IEnumerable<Album> topAlbums,
			ContentLanguagePreference languagePreference) {

			ActivityEntries = activityEntries.Select(e => new ActivityEntryContract(e, languagePreference)).ToArray();
			NewAlbums = newAlbums.Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).ToArray();
			NewsEntries = newsEntries.Select(e => new NewsEntryContract(e)).ToArray();
			TopAlbums = topAlbums.Select(a => new AlbumWithAdditionalNamesContract(a, languagePreference)).ToArray();

		}

		public ActivityEntryContract[] ActivityEntries { get; set; }

		public AlbumWithAdditionalNamesContract[] NewAlbums { get; set; }	

		public NewsEntryContract[] NewsEntries { get; set; }

		public AlbumWithAdditionalNamesContract[] TopAlbums { get; set; }

	}
}
