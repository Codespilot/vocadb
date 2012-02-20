using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() {}

		public SongListDetailsContract(SongList list, IUserPermissionContext permissionContext)
			: base(list, permissionContext) {

			SongLinks = list.SongLinks.Select(s => new SongInListContract(s, permissionContext.LanguagePreference)).ToArray();

		}

		[DataMember]
		public SongInListContract[] SongLinks { get; set; }

	}

}
