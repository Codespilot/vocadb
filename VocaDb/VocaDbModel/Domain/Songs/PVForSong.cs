namespace VocaDb.Model.Domain.Songs {

	public class PVForSong {

		private string name;
		private string pvId;
		private Song song;

		public PVForSong() {
			Name = string.Empty;
			pvId = string.Empty;
			Service = PVService.Youtube;
			PVType = PVType.Other;
		}

		public PVForSong(Song song, PVService service, string pvId, PVType pvType)
			: this() {

			Song = song;
			Service = service;
			PVId = pvId;
			PVType = pvType;

		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNull(() => value);
				name = value;
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

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual PVType PVType { get; set; }

		public virtual string Url {
			get {

				switch (Service) {
					case PVService.Youtube:
						return "http://youtu.be/" + pvId;
					case PVService.NicoNicoDouga:
						return "http://nicovideo.jp/watch/" + pvId;
					default:
						return pvId;
				}

			}
		}

		public virtual void OnDelete() {

			Song.PVs.Remove(this);
			Song.UpdateNicoId();

		}

		public override string ToString() {
			return "PV '" + Name + "' (" + Id + ") for " + Song;
		}

	}
}
