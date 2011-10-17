using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistDetailsContract : ArtistWithAdditionalNamesContract {

		public ArtistDetailsContract() {}

		public ArtistDetailsContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			AlbumLinks = artist.Albums.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Album.Name).ToArray();
			AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
			Circle = (artist.Circle != null ? new ArtistContract(artist.Circle, languagePreference) : null);
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestSongs = new SongWithAdditionalNamesContract[] { };
			Members = artist.Members.Select(m => new ArtistContract(m, languagePreference)).OrderBy(a => a.Name).ToArray();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		[DataMember]
		public ArtistForAlbumContract[] AlbumLinks { get; set; }

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public ArtistContract Circle { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArtistContract[] Members { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract[] LatestSongs { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
