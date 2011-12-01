using VocaDb.Model.Domain.Security;
using System.Xml.Serialization;
using VocaDb.Model.DataContracts.Songs;
using System.Xml.Linq;
using System.IO;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Songs {

	public class ArchivedSongVersion : ArchivedObjectVersion {

		public static ArchivedSongVersion Create(Song song, SongDiff diff, AgentLoginData author, SongArchiveReason reason, string notes) {

			var contract = new ArchivedSongContract(song, diff);
			var data = XmlHelper.SerializeToXml(contract);

			return song.CreateArchivedVersion(data, diff, author, reason, notes);

		}

		private SongDiff diff;
		private Song song;

		public ArchivedSongVersion() { }

		public ArchivedSongVersion(Song song, XDocument data, SongDiff diff, AgentLoginData author, int version, EntryStatus status, 
			SongArchiveReason reason, string notes)
			: base(data, author, version, status, notes) {

			Song = song;
			Diff = diff;
			Reason = reason;

		}

		public virtual SongDiff Diff {
			get { return diff; }
			protected set { diff = value; }
		}

		public virtual SongArchiveReason Reason { get; set; }

		public virtual Song Song {
			get { return song; }
			protected set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

	}

}
