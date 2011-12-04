using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Web.Services {

	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
	[ServiceContract(Namespace = Schemas.VocaDb)]
	public class QueryService {

		private ServiceModel Services {
			get {
				return MvcApplication.Services;
			}
		}

		#region Common queries
		[OperationContract]
		public ArtistWithAdditionalNamesContract[] FindArtists(string term, int maxResults) {

			return Services.Artists.FindArtists(term, new ArtistType[] {}, 0, maxResults, false, false).Items;

		}

		[OperationContract]
		public AlbumWithAdditionalNamesContract GetAlbumDetails(string term) {

			var albums = Services.Albums.Find(term, 0, 1, false, false);
			return albums.Items.FirstOrDefault();

		}

		[OperationContract]
		public AlbumWithAdditionalNamesContract GetAlbumById(int id) {

			var album = Services.Albums.GetAlbumWithAdditionalNames(id);
			return album;

		}

		[OperationContract]
		public ArtistDetailsContract GetArtistDetails(string term) {

			var artists = Services.Artists.FindArtists(term, new ArtistType[] {}, 0, 1, false, false);

			if (!artists.Items.Any())
				return null;

			return Services.Artists.GetArtistDetails(artists.Items[0].Id);

		}

		[OperationContract]
		public ArtistWithAdditionalNamesContract GetArtistById(int id) {

			var artist = Services.Artists.GetArtistWithAdditionalNames(id);
			return artist;

		}

		[OperationContract]
		public SongWithAdditionalNamesContract GetSongById(int id) {

			var song = Services.Songs.GetSongWithAdditionalNames(id);
			return song;

		}
		#endregion

		#region MikuDB-specific queries (TODO: move elsewhere)
		[OperationContract]
		public LyricsForSongContract GetRandomSongLyrics() {

			return Services.Songs.GetRandomSongWithLyricsDetails();

		}
		#endregion

	}
}
