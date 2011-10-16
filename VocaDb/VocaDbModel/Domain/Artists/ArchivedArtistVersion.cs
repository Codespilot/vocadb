using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists {

	public class ArchivedArtistVersion : ArchivedObjectVersion {

		public static ArchivedArtistVersion Create(Artist artist, AgentLoginData author) {

			var contract = new ArchivedArtistContract(artist);
			var doc = XmlHelper.SerializeToXml(contract);

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
