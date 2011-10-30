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
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			LatestSongs = new SongWithAdditionalNamesContract[] { };
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public ArtistForAlbumContract[] AlbumLinks { get; set; }

		[DataMember]
		public string AllNames { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public GroupForArtistContract[] Members { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract[] LatestSongs { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
