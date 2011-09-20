using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Services {

	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
	[ServiceContract]
	public class QueryService {

		private ServiceModel Services {
			get {
				return MvcApplication.Services;
			}
		}

		[OperationContract]
		public ArtistDetailsContract[] FindArtists(string term, int maxResults) {

			return Services.Artists.FindArtists(term, maxResults);

		}

		[OperationContract]
		public LyricsForSongContract GetRandomSongLyrics() {

			return Services.Songs.GetRandomSongWithLyricsDetails();

		}

	}
}
