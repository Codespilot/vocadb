using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() {}

		public SongListDetailsContract(SongList songList, ContentLanguagePreference languagePreference)
			: base(songList) {

			Author = new UserContract(songList.Author);
			Songs = songList.SongLinks.Select(s => new SongWithAdditionalNamesContract(s.Song, languagePreference)).ToArray();

		}

		public UserContract Author { get; set; }

		public SongWithAdditionalNamesContract[] Songs { get; set; }

	}

}
