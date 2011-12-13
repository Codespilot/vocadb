using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using System;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists {

	public class Artist : IEntryBase, IEntryWithNames, IEquatable<Artist>, INameFactory<ArtistName>, IWebLinkFactory<ArtistWebLink> {

		private IList<ArtistForAlbum> albums = new List<ArtistForAlbum>();
		private ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields>();
		private IList<ArtistComment> comments = new List<ArtistComment>();
		private string description;
		private IList<GroupForArtist> groups = new List<GroupForArtist>();
		private IList<GroupForArtist> members = new List<GroupForArtist>();
		private NameManager<ArtistName> names = new NameManager<ArtistName>();
		private IList<ArtistForSong> songs = new List<ArtistForSong>();
		private TagManager<ArtistTagUsage> tags = new TagManager<ArtistTagUsage>();
		private IList<ArtistWebLink> webLinks = new List<ArtistWebLink>();

		public Artist() {
			ArtistType = ArtistType.Unknown;
			Deleted = false;
			Description = string.Empty;
			StartDate = null;
			Status = EntryStatus.Draft;
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

		public virtual ArchivedVersionManager<ArchivedArtistVersion, ArtistEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual ArtistType ArtistType { get; set; }

		public virtual IList<ArtistComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual bool Deleted { get; set; }

		public virtual string DefaultName {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual EntryType EntryType {
			get {
				return EntryType.Artist;
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

		public virtual IEnumerable<GroupForArtist> Members {
			get { return AllMembers.Where(m => !m.Member.Deleted); }
		}

		[Obsolete]
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

		INameManager IEntryWithNames.Names {
			get { return Names; }
		}

		public virtual PictureData Picture { get; set; }

		public virtual IEnumerable<ArtistForSong> Songs {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public virtual DateTime? StartDate { get; set; }

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<ArtistTagUsage> Tags {
			get { return tags; }
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

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

			// Check is too slow for now
			//if (HasAlbum(album))
			//	throw new InvalidOperationException(string.Format("{0} has already been added for {1}", album, this));

			var link = new ArtistForAlbum(album, this);
			AllAlbums.Add(link);
			album.AllArtists.Add(link);

			return link;

		}

		public virtual GroupForArtist AddGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			var link = new GroupForArtist(grp, this);
			AllGroups.Add(link);
			grp.AllMembers.Add(link);

			return link;

		}

		public virtual GroupForArtist AddMember(Artist member) {

			ParamIs.NotNull(() => member);

			return member.AddGroup(this);

		}

		public virtual ArtistForSong AddSong(Song song) {

			ParamIs.NotNull(() => song);

			var link = new ArtistForSong(song, this);
			AllSongs.Add(link);
			song.AllArtists.Add(link);

			return link;

		}

		public virtual ArchivedArtistVersion CreateArchivedVersion(XDocument data, ArtistDiff diff, AgentLoginData author, ArtistArchiveReason reason, string notes) {

			var archived = new ArchivedArtistVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual ArtistComment CreateComment(string message, User author) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => author);

			var comment = new ArtistComment(this, message, author);
			Comments.Add(comment);

			return comment;

		}

		public virtual ArtistName CreateName(string val, ContentLanguageSelection language) {
			
			ParamIs.NotNullOrEmpty(() => val);

			var name = new ArtistName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual ArtistWebLink CreateWebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrWhiteSpace(() => url);

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

		public virtual ArchivedArtistVersion GetLatestVersion() {
			return ArchivedVersionsManager.GetLatestVersion();
		}

		/// <summary>
		/// Checks whether this artist has a specific album.
		/// </summary>
		/// <param name="album">Album to be checked. Cannot be null.</param>
		/// <returns>True if the artist has the album. Otherwise false.</returns>
		/// <remarks>
		/// This check can be slow if the artist has too many albums and the collection needs to be loaded.
		/// </remarks>
		public virtual bool HasAlbum(Album album) {

			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));

		}

		public virtual bool HasGroup(Artist grp) {

			ParamIs.NotNull(() => grp);

			return Groups.Any(a => a.Group.Equals(grp));

		}

		public virtual bool HasMember(Artist member) {

			ParamIs.NotNull(() => member);

			return Members.Any(a => a.Member.Equals(member));

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

		public virtual CollectionDiff<ArtistForAlbum, ArtistForAlbum> SyncAlbums(
			IEnumerable<AlbumForArtistEditContract> newAlbums, Func<AlbumForArtistEditContract, Album> albumGetter) {

			var albumDiff = CollectionHelper.Diff(Albums, newAlbums, (a1, a2) => (a1.Id == a2.ArtistForAlbumId));
			var created = new List<ArtistForAlbum>();

			foreach (var removed in albumDiff.Removed) {
				removed.Delete();
				AllAlbums.Remove(removed);
			}

			foreach (var added in albumDiff.Added) {

				var album = albumGetter(added);
				var link = AddAlbum(album);
				created.Add(link);

			}

			return new CollectionDiff<ArtistForAlbum, ArtistForAlbum>(created, albumDiff.Removed, albumDiff.Unchanged);

		}

		public override string ToString() {
			return string.Format("artist '{0}' [{1}]", DefaultName, Id);
		}

	}

}
