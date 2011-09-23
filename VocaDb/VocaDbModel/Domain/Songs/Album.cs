using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class Album {

		private IList<ArtistForSong> artists = new List<ArtistForSong>();
		private string description;
		private IList<SongInAlbum> songs = new List<SongInAlbum>();

		public Album() {
			Description = string.Empty;
			TranslatedName = new TranslatedString();
		}

		public virtual IList<ArtistForSong> Artists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual PictureData CoverPicture { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual TranslatedString TranslatedName { get; set; }

		public virtual string Name {
			get {
				return TranslatedName.Current;
			}
			set {
				TranslatedName.Current = value;
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
