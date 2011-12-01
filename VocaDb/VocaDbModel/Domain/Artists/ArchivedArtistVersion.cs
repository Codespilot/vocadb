using System.Xml.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists {

	public class ArchivedArtistVersion : ArchivedObjectVersion {

		public static ArchivedArtistVersion Create(Artist artist, ArtistDiff diff, AgentLoginData author, ArtistArchiveReason reason, string notes) {

			var contract = new ArchivedArtistContract(artist, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return artist.CreateArchivedVersion(data, diff, author, reason, notes);

		}

		private Artist artist;
		private ArtistDiff diff;

		public ArchivedArtistVersion() {}

		public ArchivedArtistVersion(Artist artist, XDocument data, ArtistDiff diff, AgentLoginData author, int version, EntryStatus status, 
			ArtistArchiveReason reason, string notes)
			: base(data, author, version, status, notes) {

			Artist = artist;
			Diff = diff;
			Reason = reason;

		}

		public virtual Artist Artist {
			get { return artist; }
			protected set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual ArtistDiff Diff {
			get { return diff; }
			protected set { diff = value; }
		}

		public virtual ArtistArchiveReason Reason { get; set; }

	}
}
