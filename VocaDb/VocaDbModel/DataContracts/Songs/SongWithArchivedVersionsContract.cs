using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongWithArchivedVersionsContract : SongContract {

		public SongWithArchivedVersionsContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference) {
			
			ParamIs.NotNull(() => song);

			ArchivedVersions = song.ArchivedVersions.Select(a => new ArchivedObjectVersionContract(a)).OrderByDescending(v => v.Version).ToArray();
			//Author = (ArchivedVersions.Any() && ArchivedVersions.Last().Author != null ? ArchivedVersions.Last().Author : null);

		}

		public ArchivedObjectVersionContract[] ArchivedVersions { get; set; }

		//public UserContract Author { get; set; }

	}

}
