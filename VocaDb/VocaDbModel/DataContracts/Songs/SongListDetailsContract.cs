using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() { }

		public SongListDetailsContract(SongList songList, ContentLanguagePreference languagePreference)
			: base(songList) {

			Author = new UserContract(songList.Author);
			SongLinks = songList.SongLinks.Select(s => new SongInListContract(s, languagePreference)).ToArray();

		}

		public UserContract Author { get; set; }

		public SongInListContract[] SongLinks { get; set; }

	}

}
