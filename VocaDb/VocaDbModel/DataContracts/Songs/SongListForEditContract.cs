using System.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListForEditContract : SongListContract {

		public SongListForEditContract() {
			SongLinks = new SongInListEditContract[] {};
		}

		public SongListForEditContract(SongList songList, IUserPermissionContext permissionContext)
			: base(songList, permissionContext) {

			SongLinks = songList.SongLinks.Select(s => new SongInListEditContract(s, permissionContext.LanguagePreference)).ToArray();

		}


		public SongInListEditContract[] SongLinks { get; set; }

	}

}
