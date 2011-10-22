using System;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class LyricsForSong : LocalizedString, IEquatable<LyricsForSong> {

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

		public virtual bool Equals(LyricsForSong another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as LyricsForSong);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

}
