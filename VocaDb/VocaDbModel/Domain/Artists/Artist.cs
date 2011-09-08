using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Artists {

	public class Artist {

		private string description;
		private IList<Artist> members = new List<Artist>();
		private IList<ArtistMetadataEntry> metadata = new List<ArtistMetadataEntry>();
		private IList<ArtistName> names = new List<ArtistName>();
		private IList<ArtistForSong> songs = new List<ArtistForSong>();
		private IList<ArtistWebLink> webLinks = new List<ArtistWebLink>();

		public Artist() {
			ArtistType = ArtistType.Unknown;
			Description = string.Empty;
			TranslatedName = new TranslatedString();
		}

		public Artist(TranslatedString translatedName)
			: this() {

			TranslatedName = translatedName;
			
			foreach (var name in translatedName.AllLocalized)
				Names.Add(new ArtistName(this, name));

		}

		public virtual IEnumerable<Album> Albums {
			get {

				return Songs.SelectMany(s => s.Song.Albums.Select(a => a.Album)).Distinct();

			}
		}

		public virtual ArtistType ArtistType { get; set; }

		public virtual Artist Circle { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual TranslatedString TranslatedName { get; set; }

		public virtual IList<Artist> Members {
			get { return members; }
			set {
				ParamIs.NotNull(() => value);
				members = value;
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
				return TranslatedName.Current;
			}
			set {
				TranslatedName.Current = value;
			}
		}

		public virtual IList<ArtistName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual PictureData Picture { get; set; }

		public virtual IList<ArtistForSong> Songs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual IList<ArtistWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get {
				return TranslatedName.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();
			}
		}

	}

}
