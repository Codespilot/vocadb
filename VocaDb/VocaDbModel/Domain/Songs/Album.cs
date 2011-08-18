using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class Album {

		private IList<SongInAlbum> songs = new List<SongInAlbum>();

		public Album() {
			LocalizedName = new LocalizedString();
		}

		public virtual IEnumerable<Artist> Artists {
			get {
				return songs.SelectMany(s => s.Song.Artists).Select(a => a.Artist).Distinct().OrderBy(a => a.Name);
			}
		}

		public virtual int Id { get; set; }

		public virtual LocalizedString LocalizedName { get; set; }

		public virtual string Name {
			get {
				return LocalizedName.Current;
			}
			set {
				LocalizedName.Current = value;
			}
		}

		public virtual DateTime ReleaseDate { get; set; }

		public virtual IList<SongInAlbum> Songs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}
	}

}
