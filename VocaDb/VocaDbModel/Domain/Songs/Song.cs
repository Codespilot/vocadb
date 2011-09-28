using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Songs {

	public class Song {

		private IList<SongInAlbum> albums = new List<SongInAlbum>();
		private IList<ArchivedSongVersion> archivedVersions = new List<ArchivedSongVersion>();
		private IList<ArtistForSong> artists = new List<ArtistForSong>();
		private IList<LyricsForSong> lyrics = new List<LyricsForSong>();
		private IList<SongMetadataEntry> metadata = new List<SongMetadataEntry>();
		private TranslatedString name;
		private IList<SongName> names = new List<SongName>();
		private string originalName;
		private IList<SongWebLink> webLinks = new List<SongWebLink>();

		protected IEnumerable<Artist> ArtistList {
			get {
				return AllArtists.Select(a => a.Artist);
			}
		}

		public Song() {
			ArtistString = string.Empty;
			CreateDate = DateTime.Now;
			Deleted = false;
			TranslatedName = new TranslatedString();
		}

		public Song(TranslatedString translatedName, string nicoId)
			: this() {

			TranslatedName = translatedName;
			NicoId = nicoId;
			OriginalName = translatedName.Display;

		}

		public Song(SongInRankingContract contract)
			: this() {
			
			ParamIs.NotNull(() => contract);

			Name = OriginalName = contract.Name;
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
			get {
				return TranslatedName.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();
			}
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

		public virtual IList<SongMetadataEntry> Metadata {
			get { return metadata; }
			set {
				ParamIs.NotNull(() => value);
				metadata = value;
			}
		}

		public virtual string Name {
			get {
				return TranslatedName.Default;
			}
			set {
				ParamIs.NotNull(() => value);
				TranslatedName.Default = value;
			}
		}

		public virtual IList<SongName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual TranslatedString TranslatedName {
			get { return name; }
			set {
				ParamIs.NotNull(() => value);
				name = value;
			}
		}

		/// <summary>
		/// NicoNicoDouga Id for the PV (for example sm12850213). Is unique, but can be null.
		/// </summary>
		public virtual string NicoId { get; set; }

		/// <summary>
		/// Original song name. This value is generally immutable and is used for archival purposes. Cannot be null or empty.
		/// </summary>
		public virtual string OriginalName {
			get { return originalName; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				originalName = value;
			}
		}

		public virtual string URL { get; set; }

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

		public virtual ArchivedSongVersion CreateArchivedVersion(XDocument data, AgentLoginData author) {

			var archived = new ArchivedSongVersion(this, data, author, Version);
			ArchivedVersions.Add(archived);
			Version++;

			return archived;

		}

		public virtual SongName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			var name = new SongName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

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

		public virtual bool HasArtist(Artist artist) {

			return ArtistList.Contains(artist);

		}

		public virtual void UpdateArtistString() {

			var producers = ArtistList.Where(a => a.ArtistType == ArtistType.Producer).Select(m => m.Name);
			var performers = ArtistList.Where(a => a.ArtistType == ArtistType.Performer).Select(m => m.Name);

			if (producers.Any() && performers.Any())
				ArtistString = string.Format("{0} feat. {1}", string.Join(", ", producers), string.Join(", ", performers));
			else
				ArtistString = string.Join(", ", ArtistList.Select(m => m.Name));

		}

	}

}
