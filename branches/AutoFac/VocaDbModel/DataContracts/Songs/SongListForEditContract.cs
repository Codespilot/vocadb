using System.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListForEditContract : SongListContract {

		public SongListForEditContract() {
			SongLinks = new SongInListEditContract[] {};
		}

		public SongListForEditContract(SongList songList, IUserPermissionContext permissionContext, bool loadSongs = true)
			: base(songList, permissionContext) {

			if (loadSongs)
				SongLinks = songList.SongLinks
					.OrderBy(s => s.Order)
					.Select(s => new SongInListEditContract(s, permissionContext.LanguagePreference))
					.ToArray();

		}


		public SongInListEditContract[] SongLinks { get; set; }

	}

}
