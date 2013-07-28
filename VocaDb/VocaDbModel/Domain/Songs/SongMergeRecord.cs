namespace VocaDb.Model.Domain.Songs {

	public class SongMergeRecord {

		private Song source;
		private Song target;

		public SongMergeRecord() {}

		public SongMergeRecord(Song source, Song target) {
			this.Source = source;
			this.Target = target;
		}

		public virtual int Id { get; set; }

		public virtual Song Source {
			get { return source; }
			set {
				ParamIs.NotNull(() => value);
				source = value;
			}
		}

		public virtual Song Target {
			get { return target; }
			set {
				ParamIs.NotNull(() => value);
				target = value;
			}
		}
	}

}
