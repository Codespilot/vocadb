using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListContract {

		public SongListContract() {
			Name = Description = string.Empty;
		}

		public SongListContract(SongList list, IUserPermissionContext permissionContext)
			: this() {

			ParamIs.NotNull(() => list);

			Author = new UserContract(list.Author);
			CanEdit = EntryPermissionManager.CanEdit(permissionContext, list);
			Description = list.Description;
			FeaturedCategory = list.FeaturedCategory;
			Id = list.Id;
			Name = list.Name;

		}

		[DataMember]
		public UserContract Author { get; set; }

		[DataMember]
		public bool CanEdit { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public SongListFeaturedCategory FeaturedCategory { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
