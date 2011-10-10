using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class PVForSongContract {

		public PVForSongContract(PVForSong pvForSong) {

			ParamIs.NotNull(() => pvForSong);

			Id = pvForSong.Id;
			Notes = pvForSong.Notes;
			PVId = pvForSong.PVId;
			Service = pvForSong.Service;
			PVType = pvForSong.PVType;
			Url = pvForSong.Url;

		}

		public PVForSongContract() { }

		public int Id { get; set; }

		public string Notes { get; set; }

		public string PVId { get; set; }

		public PVService Service { get; set; }

		public PVType PVType { get; set; }

		public string Url { get; set; }

	}
}
