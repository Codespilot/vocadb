using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Services {

	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
	[ServiceContract(Namespace = Schemas.VocaDb)]
	public class QueryService {

		private ServiceModel Services {
			get {
				return MvcApplication.Services;
			}
		}

		[OperationContract]
		public ArtistWithAdditionalNamesContract[] FindArtists(string term, int maxResults) {

			return Services.Artists.FindArtists(term, 0, maxResults).Items;

		}

		[OperationContract]
		public LyricsForSongContract GetRandomSongLyrics() {

			return Services.Songs.GetRandomSongWithLyricsDetails();

		}

	}
}
