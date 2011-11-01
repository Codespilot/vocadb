using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Albums;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace VocaDb.Model.Domain.Albums {

	public class ArchivedAlbumVersion : ArchivedObjectVersion {

		public static ArchivedAlbumVersion Create(Album album, AlbumDiff diff, AgentLoginData author, string notes) {

			var contract = new ArchivedAlbumContract(album, diff);
			var serializer = new XmlSerializer(typeof(ArchivedAlbumContract));
			XDocument doc;

			using (var writer = new StringWriter()) {
				serializer.Serialize(writer, contract);
				doc = XDocument.Parse(writer.ToString());
			}

			/*using (var stream = new MemoryStream()) {
				serializer.Serialize(stream, contract);
				var reader = XmlReader.Create(stream);
				doc = XDocument.Load(reader);
			}*/

			/*var doc = new XDocument();
			serializer.Serialize(doc.CreateWriter(), contract);*/

			return album.CreateArchivedVersion(doc, author, notes);

		}

		private Album album;

		public ArchivedAlbumVersion() { }

		public ArchivedAlbumVersion(Album album, XDocument data, AgentLoginData author, int version, string notes)
			: base(data, author, version, notes) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			protected set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

	}

}
