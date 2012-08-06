using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AllEntriesSearchResult {

		public AllEntriesSearchResult()
			: this(new PartialFindResult<AlbumWithAdditionalNamesContract>(), 
			new PartialFindResult<ArtistWithAdditionalNamesContract>(),
			new PartialFindResult<SongWithAlbumContract>()) { 
		}

		public AllEntriesSearchResult(PartialFindResult<AlbumWithAdditionalNamesContract> albums, 
			PartialFindResult<ArtistWithAdditionalNamesContract> artists,
			PartialFindResult<SongWithAlbumContract> songs) {
			Albums = albums;
			Artists = artists;
			Songs = songs;
		}

		[DataMember]
		public PartialFindResult<AlbumWithAdditionalNamesContract> Albums { get; set; }

		[DataMember]
		public PartialFindResult<ArtistWithAdditionalNamesContract> Artists { get; set; }

		[DataMember]
		public PartialFindResult<SongWithAlbumContract> Songs { get; set; }

		public bool OnlyOneItem {
			get {
				return (Albums.Items.Length + Artists.Items.Length + Songs.Items.Length == 1);
			}
		}

	}

}
