using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListForEditContract : SongListContract {

		public SongListForEditContract() {
			SongLinks = new SongInListEditContract[] {};
		}

		public SongListForEditContract(SongList songList, ContentLanguagePreference languagePreference)
			: base(songList) {

			Author = new UserContract(songList.Author);
			SongLinks = songList.SongLinks.Select(s => new SongInListEditContract(s, languagePreference)).ToArray();

		}

		public UserContract Author { get; set; }

		public SongInListEditContract[] SongLinks { get; set; }

	}

}
