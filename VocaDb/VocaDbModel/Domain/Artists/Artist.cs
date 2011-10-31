﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using System;

namespace VocaDb.Model.Domain.Artists {

	public class Artist : IEquatable<Artist>, INameFactory<ArtistName>, IWebLinkFactory<ArtistWebLink> {

		private IList<ArtistForAlbum> albums = new List<ArtistForAlbum>();
		private IList<ArchivedArtistVersion> archivedVersions = new List<ArchivedArtistVersion>();
		private string description;
		private IList<GroupForArtist> groups = new List<GroupForArtist>();
		private IList<GroupForArtist> members = new List<GroupForArtist>();
		private NameManager<ArtistName> names = new NameManager<ArtistName>();
		private IList<ArtistForSong> songs = new List<ArtistForSong>();
		private IList<ArtistWebLink> webLinks = new List<ArtistWebLink>();

		public Artist() {
			ArtistType = ArtistType.Unknown;
			Deleted = false;
			Description = string.Empty;
			StartDate = null;
			Version = 0;
		}

		public Artist(string unspecifiedName)
			: this() {

			ParamIs.NotNullOrEmpty(() => unspecifiedName);

			Names.Add(new ArtistName(this, new LocalizedString(unspecifiedName, ContentLanguageSelection.Unspecified)));

		}

		public Artist(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

			//TranslatedName = translatedName;
			
			foreach (var name in translatedName.AllLocalized)
				Names.Add(new ArtistName(this, name));

		}

		public virtual IEnumerable<ArtistForAlbum> Albums {
			get {
				return AllAlbums.Where(a => !a.Album.Deleted);
			}
		}

		public virtual IList<ArtistForAlbum> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual IList<GroupForArtist> AllGroups {
			get { return groups; }
			set {
				ParamIs.NotNull(() => value);
				groups = value;
			}
		}

		public virtual IList<GroupForArtist> AllMembers {
			get { return members; }
			set {
				ParamIs.NotNull(() => value);
				members = value;
			}
		}

		public virtual IList<ArtistForSong> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual IList<ArchivedArtistVersion> ArchivedVersions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual ArtistType ArtistType { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual IEnumerable<GroupForArtist> Groups {
			get {
				return AllGroups.Where(g => !g.Group.Deleted);
			}
		}

		public virtual int Id { get; set; }

		public virtual TranslatedString TranslatedName {
			get { return Names.SortNames; }
		}

		/*public virtual IList<ArtistMetadataEntry> Metadata {
			get { return metadata; }
			set {
				ParamIs.NotNull(() => value);
				metadata = value;
			}
		}*/

		public virtual IEnumerable<GroupForArtist> Members {
			get { return AllMembers.Where(m => !m.Member.Deleted); }
		}

		public virtual string Name {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual NameManager<ArtistName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual PictureData Picture { get; set; }

		public virtual IEnumerable<ArtistForSong> Songs {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public virtual DateTime? StartDate { get; set; }

		public virtual int Version { get; set; }

		public virtual IList<ArtistWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get { return Names.AllValues; }
		}

		public virtual ArtistForAlbum AddAlbum(Album album) {

			ParamIs.NotNull(() => album);

			var link = new ArtistForAlbum(album, this);
			AllAlbums.Add(link);
			album.AllArtists.Add(link);

			return link;

		}

		public virtual GroupForArtist AddGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			var link = new GroupForArtist(grp, this);
			AllGroups.Add(link);

			return link;

		}

		public virtual ArtistForSong AddSong(Song song) {

			ParamIs.NotNull(() => song);

			var link = new ArtistForSong(song, this);
			AllSongs.Add(link);
			song.AllArtists.Add(link);

			return link;

		}

		public virtual ArchivedArtistVersion CreateArchivedVersion(XDocument data, AgentLoginData author, string notes) {

			var archived = new ArchivedArtistVersion(this, data, author, Version, notes);
			ArchivedVersions.Add(archived);
			Version++;

			return archived;

		}

		public virtual ArtistName CreateName(string val, ContentLanguageSelection language) {
			
			ParamIs.NotNullOrEmpty(() => val);

			var name = new ArtistName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual ArtistWebLink CreateWebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new ArtistWebLink(this, description, url);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual bool Equals(Artist another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Artist);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual bool HasAlbum(Album album) {

			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));

		}

		public virtual bool HasGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			return Groups.Any(a => a.Group.Equals(grp));

		}

		public virtual bool HasName(LocalizedString name) {

			ParamIs.NotNull(() => name);

			return Names.HasName(name);

		}

		public virtual bool HasSong(Song song) {

			ParamIs.NotNull(() => song);

			return Songs.Any(a => a.Song.Equals(song));

		}

		public virtual bool HasWebLink(string url) {

			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);

		}

		public override string ToString() {
			return string.Format("artist '{0}' [{1}]", Name, Id);
		}

	}

}
