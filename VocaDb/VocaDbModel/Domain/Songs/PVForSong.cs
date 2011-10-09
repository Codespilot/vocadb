namespace VocaDb.Model.Domain.Songs {

	public class PVForSong {

		private string notes;
		private string pvId;
		private Song song;

		public PVForSong() {
			Notes = string.Empty;
			PVId = string.Empty;
			Service = PVService.Other;
			PVType = PVType.Other;
		}

		public virtual int Id { get; set; }

		public virtual string Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		public virtual string PVId {
			get { return pvId; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				pvId = value;
			}
		}

		public virtual PVService Service { get; set; }

		public Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual PVType PVType { get; set; }

	}
}
