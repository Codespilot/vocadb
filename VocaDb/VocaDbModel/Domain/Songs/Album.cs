using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Songs {

	public class Album {

		private IList<ArtistForAlbum> artists = new List<ArtistForAlbum>();
		private string description;
		private IList<SongInAlbum> songs = new List<SongInAlbum>();

		public Album() {
			Deleted = false;
			Description = string.Empty;
			TranslatedName = new TranslatedString();
		}

		public virtual IList<ArtistForAlbum> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual IEnumerable<ArtistForAlbum> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual PictureData CoverPicture { get; set; }

		public virtual bool Deleted { get; set; }

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
