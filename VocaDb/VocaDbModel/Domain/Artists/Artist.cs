using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Artists {

	public class Artist {

		private IList<ArtistMetadataEntry> metadata = new List<ArtistMetadataEntry>();
		private IList<ArtistForSong> songs = new List<ArtistForSong>();

		public Artist() {
			ArtistType = ArtistType.Unknown;
			LocalizedName = new LocalizedString();
		}

		public Artist(LocalizedString name)
			: this() {

			LocalizedName = name;

		}

		public virtual IEnumerable<Album> Albums {
			get {

				return Songs.SelectMany(s => s.Song.Albums.Select(a => a.Album)).Distinct();

			}
		}

		public virtual ArtistType ArtistType { get; set; }

		public virtual Artist Circle { get; set; }

		public virtual int Id { get; set; }

		public virtual LocalizedString LocalizedName { get; set; }

		public virtual IEnumerable<Artist> Members {
			get {
				return Enumerable.Empty<Artist>();
			}
		}

		public virtual IList<ArtistMetadataEntry> Metadata {
			get { return metadata; }
			set {
				ParamIs.NotNull(() => value);
				metadata = value;
			}
		}

		public virtual string Name {
			get {
				return LocalizedName.Current;
			}
			set {
				LocalizedName.Current = value;
			}
		}

		public virtual IList<ArtistForSong> Songs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get {
				return LocalizedName.All
					.Concat(Metadata.Where(m => m.MetadataType == ArtistMetadataType.AlternateName).Select(m => m.Value))
					.Distinct();
			}
		}

	}

}
