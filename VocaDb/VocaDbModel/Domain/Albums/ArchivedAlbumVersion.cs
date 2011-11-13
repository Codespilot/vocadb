using VocaDb.Model.Domain.Security;
using VocaDb.Model.DataContracts.Albums;
using System.Xml.Linq;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Albums {

	public class ArchivedAlbumVersion : ArchivedObjectVersion {

		public static ArchivedAlbumVersion Create(Album album, AlbumDiffContract diff, AgentLoginData author, string notes) {

			var contract = new ArchivedAlbumContract(album, diff);

			var data = XmlHelper.SerializeToXml(contract);
			var diffXml = XmlHelper.SerializeToXml(diff);

			return album.CreateArchivedVersion(data, diffXml, author, notes);

		}

		private Album album;

		public ArchivedAlbumVersion() { }

		public ArchivedAlbumVersion(Album album, XDocument data, XDocument diff, AgentLoginData author, int version, string notes)
			: base(data, author, version, notes) {

			Album = album;
			Diff = diff;

		}

		public virtual Album Album {
			get { return album; }
			protected set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual XDocument Diff { get; set; }

	}

}
