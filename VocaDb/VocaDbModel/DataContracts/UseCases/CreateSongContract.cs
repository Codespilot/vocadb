using VocaVoter.Model.DataContracts.Songs;

namespace VocaVoter.Model.DataContracts.UseCases {

	public class CreateSongContract {

		public int? AlbumId { get; set; }

		public SongContract BasicData { get; set; }

		public int? PerformerId { get; set; }

		public int? ProducerId { get; set; }

	}

}
