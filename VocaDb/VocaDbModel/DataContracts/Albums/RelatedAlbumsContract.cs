using System.Linq;

namespace VocaDb.Model.DataContracts.Albums {

	public class RelatedAlbumsContract {

		public bool Any {
			get {
				return ArtistMatches.Any() || TagMatches.Any();
			}
		}

		public AlbumContract[] ArtistMatches { get; set; }

		public AlbumContract[] TagMatches { get; set; }

	}

}
