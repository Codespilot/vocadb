using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumDetailsContract : AlbumWithAdditionalNamesContract {

		public AlbumDetailsContract() { }

		public AlbumDetailsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Artist.Name).ToArray();
			Description = album.Description;
			Songs = album.Songs.Select(s => new SongInAlbumContract(s, languagePreference)).ToArray();
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		public string Description { get; set; }

		public SongInAlbumContract[] Songs { get; set; }

		[DataMember]
		public bool UserHasAlbum { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
