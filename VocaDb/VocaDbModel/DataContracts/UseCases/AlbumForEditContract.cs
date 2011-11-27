using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.EntryValidators;

namespace VocaDb.Model.DataContracts.UseCases {

	public class AlbumForEditContract : AlbumWithAdditionalNamesContract {

		public AlbumForEditContract() {}

		public AlbumForEditContract(Album album, IEnumerable<Artist> allLabels , ContentLanguagePreference languagePreference)
			: base(album, languagePreference) {

			AllLabels = allLabels.Select(l => new ArtistContract(l, languagePreference)).ToArray();
			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Artist.Name).ToArray();
			Deleted = album.Deleted;
			Description = album.Description;
			Names = album.Names.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			OriginalRelease = (album.OriginalRelease != null ? new AlbumReleaseContract(album.OriginalRelease, languagePreference) : null);
			PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
			Songs = album.Songs.Select(s => new SongInAlbumEditContract(s, languagePreference)).ToArray();
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			ValidationResult = AlbumValidator.Validate(album);
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		public ArtistContract[] AllLabels { get; set; }

		[DataMember]
		public ArtistForAlbumContract[] ArtistLinks { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public AlbumReleaseContract OriginalRelease { get; set; }

		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongInAlbumEditContract[] Songs { get; set; }

		public TranslatedStringContract TranslatedName { get; set; }

		public ValidationResult ValidationResult { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
