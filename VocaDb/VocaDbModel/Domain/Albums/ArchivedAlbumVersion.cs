using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Albums;
using System.Xml.Linq;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Albums {

	public class ArchivedAlbumVersion : ArchivedObjectVersion {

		public static ArchivedAlbumVersion Create(Album album, AlbumDiff diff, AgentLoginData author, AlbumArchiveReason reason, string notes) {

			var contract = new ArchivedAlbumContract(album, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return album.CreateArchivedVersion(data, diff, author, reason, notes);

		}

		private Album album;
		private AlbumDiff diff;

		public ArchivedAlbumVersion() {
			Diff = new AlbumDiff();
			Reason = AlbumArchiveReason.Unknown;
		}

		public ArchivedAlbumVersion(Album album, XDocument data, AlbumDiff diff, AgentLoginData author, int version, EntryStatus status,
			AlbumArchiveReason reason, string notes)
			: base(data, author, version, status, notes) {

			Album = album;
			Diff = diff;
			Reason = reason;

		}

		/// <summary>
		/// Album associated with this revision. Can be null.
		/// </summary>
		public virtual Album Album {
			get { return album; }
			protected set {
				album = value;
			}
		}

		public virtual AlbumDiff Diff {
			get { return diff; }
			protected set { diff = value; }
		}

		public virtual AlbumArchiveReason Reason { get; set; }

	}

}
