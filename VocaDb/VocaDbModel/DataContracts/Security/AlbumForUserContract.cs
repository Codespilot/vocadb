using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Security {

	public class AlbumForUserContract {

		public AlbumForUserContract(AlbumForUser albumForUser, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => albumForUser);

			Album = new AlbumContract(albumForUser.Album, languagePreference);
			Id = albumForUser.Id;
			User = new UserContract(albumForUser.User);

		}

		[DataMember]
		public AlbumContract Album { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public UserContract User { get; set; }

	}

}
