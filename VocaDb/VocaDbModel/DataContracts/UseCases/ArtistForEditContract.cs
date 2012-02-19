using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.EntryValidators;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEditContract : ArtistWithAdditionalNamesContract {

		public const int MaxAlbums = 200;

		public ArtistForEditContract() { }

		public ArtistForEditContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			if (artist.Albums.Count() <= MaxAlbums)
				AlbumLinks = artist.Albums.Select(a => new AlbumForArtistEditContract(a, languagePreference)).OrderBy(a => a.AlbumName).ToArray();
			else {
				TooManyAlbums = true;
			}

			AllNames = string.Join(", ", artist.AllNames.Where(n => n != Name));
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			TranslatedName = new TranslatedStringContract(artist.TranslatedName);
			Members = artist.Members.Select(m => new GroupForArtistContract(m, languagePreference)).OrderBy(a => a.Member.Name).ToArray();
			Names = artist.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			ValidationResult = ArtistValidator.Validate(artist);
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public AlbumForArtistEditContract[] AlbumLinks { get; set; }

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
		public bool TooManyAlbums { get; set; }

		public ValidationResult ValidationResult { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
