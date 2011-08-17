using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	public class CreateSongContract {

		public int? AlbumId { get; set; }

		public SongContract BasicData { get; set; }

		public int? PerformerId { get; set; }

		public int? ProducerId { get; set; }

	}

}
