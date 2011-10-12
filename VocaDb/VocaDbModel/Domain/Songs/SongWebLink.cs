namespace VocaDb.Model.Domain.Songs {

	public class SongWebLink : WebLink {

		private Song song;

		public SongWebLink() { }

		public SongWebLink(Song song, string description, string url)
			: base(description, url) {

			Song = song;

		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public override string ToString() {
			return base.ToString() + " for " + Song;
		}

	}

}
