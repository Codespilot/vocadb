using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Albums {

	public class Album {

		private IList<ArtistForAlbum> artists = new List<ArtistForAlbum>();
		private string description;
		private IList<AlbumName> names = new List<AlbumName>();
		private IList<SongInAlbum> songs = new List<SongInAlbum>();
		private IList<AlbumWebLink> webLinks = new List<AlbumWebLink>();

		public Album() {
			Deleted = false;
			Description = string.Empty;
			DiscType = DiscType.Album;
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

		public virtual DiscType DiscType { get; set; }

		public virtual int Id { get; set; }

		public virtual TranslatedString TranslatedName { get; set; }

		public virtual string Name {
			get {
				return TranslatedName.Default;
			}
			set {
				TranslatedName.Default = value;
			}
		}

		public virtual IList<AlbumName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
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

		public virtual IList<AlbumWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

	}

}
