using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class LyricsForSong : LocalizedString {

		private string notes;
		private Song song;
		private string source;

		public LyricsForSong() {}

		public LyricsForSong(Song song, ContentLanguageSelection language, string val, string source)
			: base(val, language) {

			Song = song;
			Source = source;

		}

		public virtual int Id { get; protected set; }

		public virtual string Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);				
				notes = value;
			}
		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public virtual string Source {
			get { return source; }
			set {
				ParamIs.NotNull(() => value);
				source = value;
			}
		}

	}

}
