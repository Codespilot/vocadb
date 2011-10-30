using VocaDb.Model.Domain.Security;
using System.Xml.Serialization;
using VocaDb.Model.DataContracts.Songs;
using System.Xml.Linq;
using System.IO;

namespace VocaDb.Model.Domain.Songs {

	public class ArchivedSongVersion : ArchivedObjectVersion {

		public static ArchivedSongVersion Create(Song song, AgentLoginData author, string notes) {

			var contract = new ArchivedSongContract(song);
			var serializer = new XmlSerializer(typeof(ArchivedSongContract));
			XDocument doc;

			using (var writer = new StringWriter()) {
				serializer.Serialize(writer, contract);
				doc = XDocument.Parse(writer.ToString());
			}

			return song.CreateArchivedVersion(doc, author, notes);

		}

		private Song song;

		public ArchivedSongVersion() { }

		public ArchivedSongVersion(Song song, XDocument data, AgentLoginData author, int version, string notes)
			: base(data, author, version, notes) {

			Song = song;

		}

		public virtual Song Song {
			get { return song; }
			protected set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

	}

}
