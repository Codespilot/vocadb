using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumDetailsContract : AlbumContract {

		public AlbumDetailsContract() { }

		public AlbumDetailsContract(Album album, ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			AdditionalNames = string.Join(", ", album.TranslatedName.All.Where(n => n != album.Name));
			Artists = album.AllArtists.Select(a => new ArtistContract(a.Artist, languagePreference)).ToArray();
			Description = album.Description;
			Songs = album.Songs.Select(s => new SongInAlbumContract(s)).ToArray();
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		[DataMember]
		public string AdditionalNames { get; set; }

		public ArtistContract[] Artists { get; set; }

		public string Description { get; set; }

		public SongInAlbumContract[] Songs { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
