using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class LyricsForSong {

		private string notes;
		private Song song;
		private string text;

		public virtual int Id { get; protected set; }

		public virtual ContentLanguageSelection Language { get; protected set; }

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

		public virtual string Text {
			get { return text; }
			set {
				ParamIs.NotNull(() => value);
				text = value;
			}
		}
	}

}
