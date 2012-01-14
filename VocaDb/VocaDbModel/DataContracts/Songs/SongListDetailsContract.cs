using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() {}

		public SongListDetailsContract(SongList songList, ContentLanguagePreference languagePreference)
			: base(songList) {

			SongLinks = songList.SongLinks.Select(s => new SongInListContract(s, languagePreference)).ToArray();

		}

		[DataMember]
		public SongInListContract[] SongLinks { get; set; }

	}

}
