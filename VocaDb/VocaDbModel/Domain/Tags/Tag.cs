using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag>, IEntryBase {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		int IEntryWithIntId.Id {
			get { return GetHashCode(); }
		}

		public const int MaxDisplayedTags = 4;
		public static readonly Regex TagNameRegex = new Regex(@"^[a-zA-Z0-9_-]+$");

		private Iesi.Collections.Generic.ISet<AlbumTagUsage> albumTagUsages = new Iesi.Collections.Generic.HashedSet<AlbumTagUsage>();
		private Iesi.Collections.Generic.ISet<ArtistTagUsage> artistTagUsages = new Iesi.Collections.Generic.HashedSet<ArtistTagUsage>();
		private string categoryName;
		private string description;
		private Iesi.Collections.Generic.ISet<SongTagUsage> songTagUsages = new Iesi.Collections.Generic.HashedSet<SongTagUsage>();

		public Tag() {
			CategoryName = string.Empty;
			Description = string.Empty;
		}

		public Tag(string name)
			: this() {

			if (!TagNameRegex.IsMatch(name))
				throw new ArgumentException("Tag name must contain only word characters", "name");

			Name = name;

		}

		public virtual Iesi.Collections.Generic.ISet<AlbumTagUsage> AllAlbumTagUsages {
			get { return albumTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				albumTagUsages = value;
			}
		}

		public virtual IEnumerable<AlbumTagUsage> AlbumTagUsages {
			get {
				return AllAlbumTagUsages.Where(a => !a.Album.Deleted);
			}
		}

		public virtual Iesi.Collections.Generic.ISet<ArtistTagUsage> AllArtistTagUsages {
			get { return artistTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				artistTagUsages = value;
			}
		}

		public virtual IEnumerable<ArtistTagUsage> ArtistTagUsages {
			get {
				return AllArtistTagUsages.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string CategoryName {
			get { return categoryName; }
			set {
				ParamIs.NotNull(() => value);
				categoryName = value; 
			}
		}

		public virtual string Description {
			get { return description; }
			set { description = value; }
		}

		public virtual EntryType EntryType {
			get { return EntryType.Tag; }
		}

		public virtual string Name { get; set; }

		public virtual string TagName { get; set; }

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			return tag.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Name.ToLowerInvariant().GetHashCode();
		}

		public virtual Iesi.Collections.Generic.ISet<SongTagUsage> AllSongTagUsages {
			get { return songTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				songTagUsages = value;
			}
		}

		public virtual IEnumerable<SongTagUsage> SongTagUsages {
			get {
				return AllSongTagUsages.Where(a => !a.Song.Deleted);
			}
		}

		public override string ToString() {
			return string.Format("tag '{0}'", Name);
		}

	}

}
