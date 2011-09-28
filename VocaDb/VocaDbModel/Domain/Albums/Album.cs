using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Albums {

	public class Album {

		private IList<ArchivedAlbumVersion> archivedVersions = new List<ArchivedAlbumVersion>();
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

		public Album(TranslatedString translatedName)
			: this() {

			TranslatedName = translatedName;
			
			foreach (var name in translatedName.AllLocalized)
				Names.Add(new AlbumName(this, name));

		}

		public virtual IList<ArtistForAlbum> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual IList<ArchivedAlbumVersion> ArchivedVersions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
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

		public virtual int Version { get; set; }

		public virtual IList<AlbumWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual ArchivedAlbumVersion CreateArchivedVersion(XDocument data, AgentLoginData author) {

			var archived = new ArchivedAlbumVersion(this, data, author, Version);
			ArchivedVersions.Add(archived);
			Version++;

			return archived;

		}

		public virtual AlbumName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			var name = new AlbumName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual AlbumWebLink CreateWebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new AlbumWebLink(this, description, url);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

	}

}
