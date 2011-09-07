using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class LyricsForSong : LocalizedString {

		private string notes;
		private Song song;

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

	}

}
