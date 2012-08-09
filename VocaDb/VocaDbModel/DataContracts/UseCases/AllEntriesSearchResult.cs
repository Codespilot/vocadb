using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AllEntriesSearchResult {

		public AllEntriesSearchResult()
			: this(string.Empty,
			new PartialFindResult<AlbumWithAdditionalNamesContract>(), 
			new PartialFindResult<ArtistWithAdditionalNamesContract>(),
			new PartialFindResult<SongWithAlbumContract>(),
			new PartialFindResult<TagContract>()) { 
		}

		public AllEntriesSearchResult(string query, 
			PartialFindResult<AlbumWithAdditionalNamesContract> albums, 
			PartialFindResult<ArtistWithAdditionalNamesContract> artists,
			PartialFindResult<SongWithAlbumContract> songs,
			PartialFindResult<TagContract> tags) {

			Query = query;
			Albums = albums;
			Artists = artists;
			Songs = songs;
			Tags = tags;

		}

		[DataMember]
		public PartialFindResult<AlbumWithAdditionalNamesContract> Albums { get; set; }

		[DataMember]
		public PartialFindResult<ArtistWithAdditionalNamesContract> Artists { get; set; }

		[DataMember]
		public PartialFindResult<SongWithAlbumContract> Songs { get; set; }

		[DataMember]
		public string Query { get; set; }

		[DataMember]
		public PartialFindResult<TagContract> Tags { get; set; }

		public bool OnlyOneItem {
			get {
				return (Albums.Items.Length + Artists.Items.Length + Songs.Items.Length + Tags.Items.Length == 1);
			}
		}

	}

}
