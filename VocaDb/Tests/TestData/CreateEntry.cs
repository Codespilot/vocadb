using NHibernate.Util;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestData {

	public static class CreateEntry {

		public static Album Album(int id = 0, string name = "Synthesis") {
			return new Album(new LocalizedString(name, ContentLanguageSelection.Unspecified)) { Id = id };
		}

		public static Artist Producer(int id = 0, string name = null) {
			return new Artist(TranslatedString.Create(name ?? "Tripshots")) { Id = id, ArtistType = ArtistType.Producer };
		}

		public static Song Song(int id = 0, string name = null) {
			return new Song(TranslatedString.Create(name ?? "Nebula")) { Id = id };
		}

		public static User User(int id = 0, string name = "Miku", UserGroupId group = UserGroupId.Regular, string email = "") {
			return new User(name, "123", email, 0) { GroupId = group, Id = id };
		}

		public static UserMessage UserMessage(int id = 0, User sender = null, User receiver = null, 
			string subject = "Hello world", string body = "Message body", bool highPriority = false, 
			bool read = false) {

			return new UserMessage(sender, receiver, subject, body, highPriority) { Id = id, Read = read };

		}

		public static Artist Vocalist(int id = 0, string name = null, ArtistType artistType = ArtistType.Vocaloid) {
			return new Artist(TranslatedString.Create(name ?? "Hatsune Miku")) { Id = id, ArtistType = artistType };			
		}

	}

}
