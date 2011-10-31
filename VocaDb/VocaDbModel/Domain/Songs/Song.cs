﻿using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class Song : INameFactory<SongName>, IWebLinkFactory<SongWebLink> {

		private IList<SongInAlbum> albums = new List<SongInAlbum>();
		private IList<ArchivedSongVersion> archivedVersions = new List<ArchivedSongVersion>();
		private IList<ArtistForSong> artists = new List<ArtistForSong>();
		private IList<LyricsForSong> lyrics = new List<LyricsForSong>();
		private NameManager<SongName> names = new NameManager<SongName>();
		private IList<PVForSong> pvs = new List<PVForSong>();
		private IList<FavoriteSongForUser> userFavorites = new List<FavoriteSongForUser>();
		private IList<SongWebLink> webLinks = new List<SongWebLink>();

		protected IEnumerable<Artist> ArtistList {
			get {
				return Artists.Select(a => a.Artist);
			}
		}

		public Song() {
			ArtistString = string.Empty;
			CreateDate = DateTime.Now;
			Deleted = false;
			SongType = SongType.Unspecified;
		}

		public Song(string unspecifiedName)
			: this() {

			ParamIs.NotNullOrEmpty(() => unspecifiedName);

			Names.Add(new SongName(this, new LocalizedString(unspecifiedName, ContentLanguageSelection.Unspecified)));

		}

		public Song(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

			//TranslatedName = translatedName;

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

		public virtual IList<ArchivedSongVersion> ArchivedVersions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistForSong> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string ArtistString { get; protected set; }

		public virtual DateTime CreateDate { get; protected set; }

		public virtual bool Deleted { get; set; }

		public virtual int Id { get; set; }

		public virtual IList<LyricsForSong> Lyrics {
			get { return lyrics; }
			set {
				ParamIs.NotNull(() => value);
				lyrics = value;
			}
		}

		/*public virtual IList<SongMetadataEntry> Metadata {
			get { return metadata; }
			set {
				ParamIs.NotNull(() => value);
				metadata = value;
			}
		}*/

		public virtual string DefaultName {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual NameManager<SongName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual IList<PVForSong> PVs {
			get { return pvs; }
			set {
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		public virtual SongType SongType { get; set; }

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

		public virtual SongInAlbum AddAlbum(Album album, int trackNumber) {

			var link = new SongInAlbum(this, album, trackNumber);
			AllAlbums.Add(link);
			return link;

		}

		public virtual ArtistForSong AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var link = new ArtistForSong(this, artist);
			AllArtists.Add(link);
			return link;

		}

		public virtual ArchivedSongVersion CreateArchivedVersion(XDocument data, AgentLoginData author, string notes) {

			var archived = new ArchivedSongVersion(this, data, author, Version, notes);
			ArchivedVersions.Add(archived);
			Version++;

			return archived;

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

			var name = new SongName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual PVForSong CreatePV(PVService service, string pvId, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvId);

			var pv = new PVForSong(this, service, pvId, pvType);
			PVs.Add(pv);
			return pv;

		}

		public virtual SongWebLink CreateWebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new SongWebLink(this, description, url);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual void DeleteArtistForSong(ArtistForSong artistForSong) {

			if (!artistForSong.Song.Equals(this))
				throw new ArgumentException("Artist is not attached to song", "artistForSong");

			AllArtists.Remove(artistForSong);
			UpdateArtistString();

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

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual bool HasArtist(Artist artist) {

			return ArtistList.Contains(artist);

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

		public override string ToString() {
			return string.Format("song '{0}' [{1}]", DefaultName, Id);
		}

		public virtual void UpdateArtistString() {

			ArtistString = ArtistHelper.GetArtistString(ArtistList);

		}

		public virtual void UpdateNicoId() {

			var originalPv = PVs.FirstOrDefault(p => p.Service == PVService.NicoNicoDouga && p.PVType == PVType.Original);

			NicoId = (originalPv != null ? originalPv.PVId : null);

		}

	}

}
