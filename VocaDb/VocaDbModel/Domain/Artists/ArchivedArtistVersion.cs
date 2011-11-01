using System.Xml.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Artists {

	public class ArchivedArtistVersion : ArchivedObjectVersion {

		public static ArchivedArtistVersion Create(Artist artist, ArtistDiff diff, AgentLoginData author, string notes) {

			var contract = new ArchivedArtistContract(artist, diff);
			var doc = XmlHelper.SerializeToXml(contract);

			return artist.CreateArchivedVersion(doc, author, notes);

		}

		private Artist artist;

		public ArchivedArtistVersion() {}

		public ArchivedArtistVersion(Artist artist, XDocument data, AgentLoginData author, int version, string notes)
			: base(data, author, version, notes) {

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
