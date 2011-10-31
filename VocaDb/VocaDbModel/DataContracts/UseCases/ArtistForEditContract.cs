using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEditContract : ArtistWithAdditionalNamesContract {

		public ArtistForEditContract() { }

		public ArtistForEditContract(Artist artist, ContentLanguagePreference languagePreference, IEnumerable<Artist> allCircles)
			: base(artist, languagePreference) {

			ParamIs.NotNull(() => allCircles);

			AlbumLinks = artist.Albums.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Album.Name).ToArray();
			AllCircles = allCircles.OrderBy(a => a.TranslatedName[languagePreference]).Select(a => new ArtistContract(a, languagePreference)).ToArray();
			AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			Names = artist.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public ArtistForAlbumContract[] AlbumLinks { get; set; }

		[DataMember]
		public ArtistContract[] AllCircles { get; set; }

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
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
