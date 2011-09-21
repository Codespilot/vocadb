using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {
	public class SongName : LocalizedStringWithId {

		private Song song;

		public SongName() {}

		public SongName(Song song, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language) {

			Song = song;

		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}


	}
}
