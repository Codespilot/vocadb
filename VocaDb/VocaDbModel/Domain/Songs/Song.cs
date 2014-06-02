﻿using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Xml.Linq;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.Songs {

	public class Song : IEntryBase, IEntryWithNames<SongName>, IEntryWithStatus, IDeletableEntry, INameFactory<SongName>, IWebLinkFactory<SongWebLink>, IEquatable<Song> {

		private IList<SongInAlbum> albums = new List<SongInAlbum>();
		private IList<Song> alternateVersions = new List<Song>();
		private ArchivedVersionManager<ArchivedSongVersion, SongEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedSongVersion, SongEditableFields>();
		private TranslatedStringWithDefault artistString;
		private IList<ArtistForSong> artists = new List<ArtistForSong>();
		private IList<SongComment> comments = new List<SongComment>();
		private IList<SongInList> lists = new List<SongInList>();
		private IList<LyricsForSong> lyrics = new List<LyricsForSong>();
		private NameManager<SongName> names = new NameManager<SongName>();
		private string notes;
		private PVManager<PVForSong> pvs = new PVManager<PVForSong>();
		private TagManager<SongTagUsage> tags = new TagManager<SongTagUsage>();
		private IList<FavoriteSongForUser> userFavorites = new List<FavoriteSongForUser>();
		private IList<SongWebLink> webLinks = new List<SongWebLink>();

		public virtual int GetLengthFromPV() {

			var pv = PVs.FirstOrDefault(p => p.Length > 0);
			return (pv != null ? pv.Length : 0);

		}

		public Song() {
			ArtistString = new TranslatedStringWithDefault(string.Empty, string.Empty, string.Empty, string.Empty);
			CreateDate = DateTime.Now;
			Deleted = false;
			Notes = string.Empty;
			PVServices = PVServices.Nothing;
			SongType = SongType.Unspecified;
			Status = EntryStatus.Draft;
		}

		public Song(LocalizedString name)
			: this() {

			ParamIs.NotNull(() => name);

			Names.Add(new SongName(this, name));

		}

		public Song(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

			foreach (var name in translatedName.AllLocalized)
				Names.Add(new SongName(this, name));

		}

		public Song(SongInRankingContract contract)
			: this() {
			
			ParamIs.NotNull(() => contract);

			Names.Add(new SongName(this, new LocalizedString(contract.Name, ContentLanguageSelection.Unspecified)));
			NicoId = contract.NicoId;

		}

		public virtual IEnumerable<SongInAlbum> Albums {
			get { 
				return AllAlbums.Where(a => !a.Album.Deleted); 
			}
		}

		public virtual IList<SongInAlbum> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual IList<Song> AllAlternateVersions {
			get { return alternateVersions; }
			set {
				ParamIs.NotNull(() => value);
				alternateVersions = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get { return Names.AllValues; }
		}

		public virtual IList<ArtistForSong> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual IEnumerable<Song> AlternateVersions {
			get {
				return AllAlternateVersions.Where(a => !a.Deleted);
			}
		}

		public virtual ArchivedVersionManager<ArchivedSongVersion, SongEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<Artist> ArtistList {
			get {
				return Artists
					.Where(a => a.Artist != null)
					.Select(a => a.Artist);
			}
		}

		public virtual IEnumerable<ArtistForSong> Artists {
			get {
				return AllArtists.Where(a => a.Artist == null || !a.Artist.Deleted);
			}
		}

		public virtual TranslatedStringWithDefault ArtistString {
			get { return artistString; }
			set {
				ParamIs.NotNull(() => value);
				artistString = value;
			}
		}

		public virtual IList<SongComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}
		public virtual DateTime CreateDate { get; set; }

		public virtual string DefaultName {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual bool Deleted { get; set; }

		public virtual EntryType EntryType {
			get {
				return EntryType.Song;
			}
		}

		public virtual int FavoritedTimes { get; set; }

		public virtual bool HasOriginalVersion {
			get {
				return SongType != SongType.Original && OriginalVersion != null;
			}
		}

		public virtual int Id { get; set; }

		/// <summary>
		/// Song length in seconds. If 0, that means no length is saved.
		/// </summary>
		public virtual int LengthSeconds { get; set; }

		public virtual IList<SongInList> ListLinks {
			get { return lists; }
			set {
				ParamIs.NotNull(() => value);
				lists = value;
			}
		}

		public virtual IList<LyricsForSong> Lyrics {
			get { return lyrics; }
			set {
				ParamIs.NotNull(() => value);
				lyrics = value;
			}
		}

		/// <summary>
		/// Lyrics for this song, either from the song entry itself, or its original version.
		/// </summary>
		public virtual IList<LyricsForSong> LyricsFromParents {
			get {

				if (SongType != SongType.Instrumental && HasOriginalVersion && !Lyrics.Any() && OriginalVersion.Lyrics.Any())
					return OriginalVersion.Lyrics;

				return Lyrics;

			}
		}

		public virtual NameManager<SongName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		INameManager<SongName> IEntryWithNames<SongName>.Names {
			get { return Names; }
		}

		INameManager IEntryWithNames.Names {
			get { return Names; }
		}

		public virtual string Notes {
			get { return notes; }
			set {
				ParamIs.NotNull(() => value);
				notes = value; 
			}
		}

		public virtual Song OriginalVersion { get; set; }

		public virtual PVManager<PVForSong> PVs {
			get { return pvs; }
			set {
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		public virtual PVServices PVServices { get; set; }

		public virtual int RatingScore { get; set; }

		public virtual SongType SongType { get; set; }

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<SongTagUsage> Tags {
			get { return tags; }
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		public virtual TranslatedString TranslatedName {
			get { return Names.SortNames; }
		}

		/// <summary>
		/// NicoNicoDouga Id for the PV (for example sm12850213). Is unique, but can be null.
		/// </summary>
		public virtual string NicoId { get; set; }

		public virtual IList<FavoriteSongForUser> UserFavorites {
			get { return userFavorites; }
			set {
				ParamIs.NotNull(() => value);
				userFavorites = value;
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<SongWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		/*public virtual SongInAlbum AddAlbum(Album album, int trackNumber) {

			var link = new SongInAlbum(this, album, trackNumber);
			AllAlbums.Add(link);
			return link;

		}*/

		public virtual ArtistForSong AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			return artist.AddSong(this);

		}

		public virtual ArtistForSong AddArtist(string name, bool isSupport, ArtistRoles roles) {

			ParamIs.NotNullOrEmpty(() => name);

			var link = new ArtistForSong(this, name, isSupport, roles);

			AllArtists.Add(link);

			return link;

		}
		public virtual void AddAlternateVersion(Song song) {

			ParamIs.NotNull(() => song);

			if (song.OriginalVersion != null)
				song.OriginalVersion.AllAlternateVersions.Remove(song);

			AllAlternateVersions.Add(song);
			song.OriginalVersion = this;

		}

		/// <summary>
		/// Adds a tag to the song.
		/// First checks if the tag has already been added.
		/// </summary>
		/// <param name="tag">Tag to be added. Cannot be null.</param>
		/// <returns>The created tag usage. Can be null if the tag has already been added.</returns>
		public virtual SongTagUsage AddTag(Tag tag) {
			
			ParamIs.NotNull(() => tag);

			if (Tags.HasTag(tag))
				return null;

			var usage = new SongTagUsage(this, tag);
			Tags.Usages.Add(usage);
			return usage;

		}

		public virtual ArchivedSongVersion CreateArchivedVersion(XDocument data, SongDiff diff, AgentLoginData author, SongArchiveReason reason, string notes) {

			var archived = new ArchivedSongVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual SongComment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new SongComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual LyricsForSong CreateLyrics(ContentLanguageSelection language, string val, string source) {
			
			ParamIs.NotNullOrEmpty(() => val);
			ParamIs.NotNull(() => source);

			var entry = new LyricsForSong(this, language, val, source);
			Lyrics.Add(entry);

			return entry;

		}

		public virtual SongName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			return CreateName(new LocalizedString(val, language));

		}

		public virtual SongName CreateName(LocalizedString localizedString) {

			ParamIs.NotNull(() => localizedString);

			var name = new SongName(this, localizedString);
			Names.Add(name);

			return name;

		}

		public virtual PVForSong CreatePV(PVContract contract) {

			ParamIs.NotNull(() => contract);

			var pv = new PVForSong(this, contract);
			PVs.Add(pv);

			UpdateNicoId();
			UpdatePVServices();

			if (LengthSeconds <= 0)
				LengthSeconds = GetLengthFromPV();

			return pv;

		}

		public virtual SongWebLink CreateWebLink(string description, string url, WebLinkCategory category) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new SongWebLink(this, description, url, category);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual void DeleteArtistForSong(ArtistForSong artistForSong) {

			if (!artistForSong.Song.Equals(this))
				throw new ArgumentException("Artist is not attached to song", "artistForSong");

			artistForSong.Delete();

		}

		public virtual bool Equals(Song another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Song);
		}

		public virtual ArtistForSong GetArtistLink(Artist artist) {
			return Artists.FirstOrDefault(a => a.Artist != null && a.Artist.Equals(artist));
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual ArchivedSongVersion GetLatestVersion() {
			return ArchivedVersionsManager.GetLatestVersion();
		}

		public virtual bool HasArtist(Artist artist) {

			return ArtistList.Contains(artist);

		}

		/// <summary>
		/// Checks whether this song has a specific artist.
		/// </summary>
		/// <param name="artistLink">Artist to be checked. Cannot be null.</param>
		/// <returns>True if the artist has this album. Otherwise false.</returns>
		public virtual bool HasArtistLink(ArtistForSong artistLink) {

			ParamIs.NotNull(() => artistLink);

			return Artists.Any(a => a.ArtistLinkEquals(artistLink));

		}

		public virtual bool HasName(LocalizedString name) {

			ParamIs.NotNull(() => name);

			return Names.HasName(name);

		}

		public virtual bool HasPV(PVService service, string pvId) {

			ParamIs.NotNullOrEmpty(() => pvId);

			return PVs.Any(p => p.Service == service && p.PVId == pvId);

		}

		public virtual bool HasWebLink(string url) {

			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);

		}

		public virtual bool IsOnAlbum(Album album) {

			ParamIs.NotNull(() => album);

			return Albums.Any(a => a.Album.Equals(album));

		}

		public virtual bool IsFavoritedBy(User user) {

			ParamIs.NotNull(() => user);

			return UserFavorites.Any(a => a.User.Equals(user));

		}

		public virtual ArtistForSong RemoveArtist(Artist artist) {

			var link = Artists.First(a => a.Artist.Equals(artist));

			if (link == null)
				return null;

			DeleteArtistForSong(link);

			return link;

		}

		public virtual CollectionDiff<ArtistForSong, ArtistForSong> SyncArtists(IEnumerable<ArtistContract> newArtists, Func<ArtistContract[], Artist[]> artistGetter) {

			var realArtists = Artists.Where(a => a.Artist != null).ToArray();
			var artistDiff = CollectionHelper.Diff(realArtists, newArtists, (a, a2) => a.Artist.Id == a2.Id);
			var created = new List<ArtistForSong>();

			if (artistDiff.Added.Any()) {

				var addedArtists = artistGetter(artistDiff.Added);

				foreach (var artist in addedArtists) {
					if (!HasArtist(artist)) {
						created.Add(AddArtist(artist));
					}
				}
				
			}

			foreach (var removed in artistDiff.Removed) {
				removed.Delete();
			}

			UpdateArtistString();

			return new CollectionDiff<ArtistForSong, ArtistForSong>(created, artistDiff.Removed, artistDiff.Unchanged);

		}

		public virtual CollectionDiffWithValue<ArtistForSong, ArtistForSong> SyncArtists(
			IEnumerable<ArtistForSongContract> newArtists, Func<ArtistForSongContract, Artist> artistGetter) {

			ParamIs.NotNull(() => newArtists);

			var diff = CollectionHelper.Diff(Artists, newArtists, (n1, n2) => n1.Id == n2.Id);
			var created = new List<ArtistForSong>();
			var edited = new List<ArtistForSong>();

			foreach (var n in diff.Removed) {
				n.Delete();
			}

			foreach (var newEntry in diff.Added) {

				ArtistForSong l;

				if (newEntry.Artist != null) {

					var artist = artistGetter(newEntry);

					if (!HasArtist(artist)) {
						l = artist.AddSong(this, newEntry.IsSupport, newEntry.Roles);
						created.Add(l);
					}

				} else {
					l = AddArtist(newEntry.Name, newEntry.IsSupport, newEntry.Roles);
					created.Add(l);
				}

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newArtists.First(e => e.Id == entry.Id);

				if (!linkEntry.ContentEquals(newEntry)) {
					linkEntry.IsSupport = newEntry.IsSupport;
					linkEntry.Roles = newEntry.Roles;
					edited.Add(linkEntry);
				}

			}

			UpdateArtistString();

			return new CollectionDiffWithValue<ArtistForSong, ArtistForSong>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiffWithValue<LyricsForSong, LyricsForSong> SyncLyrics(IEnumerable<LyricsForSongContract> newLyrics) {

			ParamIs.NotNull(() => newLyrics);

			var diff = CollectionHelper.Diff(Lyrics, newLyrics, (n1, n2) => n1.Id == n2.Id);
			var created = new List<LyricsForSong>();
			var edited = new List<LyricsForSong>();

			foreach (var n in diff.Removed) {
				Lyrics.Remove(n);
			}

			foreach (var newEntry in diff.Added) {

				var l = CreateLyrics(newEntry.Language, newEntry.Value, newEntry.Source);
				created.Add(l);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newLyrics.First(e => e.Id == entry.Id);

				if (!entry.ContentEquals(newEntry)) {
					linkEntry.Language = newEntry.Language;
					linkEntry.Source = newEntry.Source;
					linkEntry.Value = newEntry.Value;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<LyricsForSong, LyricsForSong>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiffWithValue<PVForSong, PVForSong> SyncPVs(IList<PVContract> newPVs) {

			return PVs.Sync(newPVs, CreatePV);

		}

		public override string ToString() {
			return string.Format("song '{0}' [{1}]", DefaultName, Id);
		}

		public virtual void UpdateArtistString() {

			ArtistString = ArtistHelper.GetArtistString(Artists, SongHelper.IsAnimation(SongType));

		}

		public virtual void UpdateFavoritedTimes() {

			FavoritedTimes = UserFavorites.Count;

		}

		public virtual void UpdateNicoId() {

			var originalPv = PVs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			NicoId = (originalPv != null ? originalPv.PVId : null);

		}

		public virtual void UpdatePVServices() {

			var services = PVServices.Nothing;

			foreach (var service in EnumVal<PVService>.Values) {
				if (PVs.Any(p => p.Service == service))
					services |= (PVServices)service;
			}

			PVServices = services;

		}

	}

}
