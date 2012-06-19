using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForAlbumContract {

		public ArtistForAlbumContract(ArtistForAlbum artistForAlbum, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => artistForAlbum);

			Album = new AlbumWithAdditionalNamesContract(artistForAlbum.Album, languagePreference);
			Artist = (artistForAlbum.Artist != null ? new ArtistWithAdditionalNamesContract(artistForAlbum.Artist, languagePreference) : null);
			Categories = artistForAlbum.ArtistCategories;
			Id = artistForAlbum.Id;
			IsSupport = artistForAlbum.IsSupport;
			Name = artistForAlbum.Name;
			Roles = artistForAlbum.Roles;

		}

		[DataMember]
		public AlbumWithAdditionalNamesContract Album { get; set; }

		[DataMember]
		public ArtistWithAdditionalNamesContract Artist { get; set; }

		[DataMember]
		public ArtistCategories Categories { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }

	}

}
