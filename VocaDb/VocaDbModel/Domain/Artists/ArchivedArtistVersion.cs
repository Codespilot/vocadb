using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Artists {

	public class ArchivedArtistVersion : ArchivedObjectVersion {

		public static ArchivedArtistVersion Create(Artist artist, AgentLoginData author) {

			var contract = new ArchivedArtistContract(artist);
			var serializer = new XmlSerializer(typeof(ArchivedArtistContract));
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

			return artist.CreateArchivedVersion(doc, author);

		}

		private Artist artist;

		public ArchivedArtistVersion() {}

		public ArchivedArtistVersion(Artist artist, XDocument data, AgentLoginData author, int version)
			: base(data, author, version) {

			Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			protected set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

	}
}
