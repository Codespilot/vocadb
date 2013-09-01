using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() {}

		public SongListDetailsContract(SongList list, PartialFindResult<SongInListContract> songLinks, IUserPermissionContext permissionContext)
			: base(list, permissionContext) {

			SongLinks = songLinks;
			//SongLinks = list.SongLinks.Select(s => new SongInListContract(s, permissionContext.LanguagePreference)).ToArray();

		}

		[DataMember]
		public PartialFindResult<SongInListContract> SongLinks { get; set; }

	}

}
