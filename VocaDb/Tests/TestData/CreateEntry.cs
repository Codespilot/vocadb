using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestData {

	public static class CreateEntry {

		public static Album Album(string name = "Synthesis") {
			return new Album(TranslatedString.Create(name)) { Id = 39 };
		}

		public static Artist Producer() {
			return new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer };
		}

		public static Song Song() {
			return new Song(TranslatedString.Create("Nebula")) { Id = 1 };
		}

		public static User User(int id = 1, string name = "Miku", UserGroupId group = UserGroupId.Regular, string email = "") {
			return new User(name, "123", email, 0) { GroupId = group, Id = id };
		}

		public static Artist Vocalist() {
			return new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39, ArtistType = ArtistType.Vocaloid };			
		}

	}

}
