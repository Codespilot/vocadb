using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistDetailsContract : ArtistWithAdditionalNamesContract {

		public ArtistDetailsContract() {}

		public ArtistDetailsContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			Albums = artist.Albums.Select(a => new AlbumContract(a.Album, languagePreference)).OrderBy(a => a.Name).ToArray();
			AllNames = string.Join(", ", artist.AllNames.Where(n => n != artist.Name));
			Circle = (artist.Circle != null ? new ArtistContract(artist.Circle, languagePreference) : null);
			Description = artist.Description;
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			Members = artist.Members.Select(m => new ArtistContract(m, languagePreference)).ToArray();
			Songs = artist.Songs.Select(s => new SongContract(s.Song)).ToArray();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		public AlbumContract[] Albums { get; set; }

		public string AllNames { get; set; }

		public ArtistContract Circle { get; set; }

		public string Description { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

		public ArtistContract[] Members { get; set; }

		public SongContract[] Songs { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
