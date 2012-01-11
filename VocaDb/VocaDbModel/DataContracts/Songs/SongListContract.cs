using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListContract {

		public SongListContract() {
			Name = Description = string.Empty;
		}

		public SongListContract(SongList list) {

			ParamIs.NotNull(() => list);

			Author = new UserContract(list.Author);
			Description = list.Description;
			Id = list.Id;
			Name = list.Name;

		}

		[DataMember]
		public UserContract Author { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
