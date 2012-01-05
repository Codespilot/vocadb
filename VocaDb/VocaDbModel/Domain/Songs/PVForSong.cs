using System;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.Songs {

	public class PVForSong : PV {

		private string author;
		private Song song;

		public PVForSong() { }

		public PVForSong(Song song, PVService service, string pvId, PVType pvType, string name)
			: base(service, pvId, pvType, name) {

			Author = string.Empty;
			Song = song;

		}

		public virtual string Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual bool Equals(PVForSong another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as PVForSong);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void OnDelete() {

			Song.PVs.Remove(this);
			Song.UpdateNicoId();
			Song.UpdatePVServices();

		}

		public override string ToString() {
			return string.Format("PV '{0}' [{1}] for {2}", PVId, Id, Song);
		}

	}
}
