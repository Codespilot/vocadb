using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.PVs;
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
		public PartialFindResult<AlbumWithAdditionalNamesContract> FindAlbums(string term, int maxResults) {

			return Services.Albums.Find(term, 0, maxResults, false, true, moveExactToTop: true);

		}

		[OperationContract]
		public PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(string term, int maxResults) {

			return Services.Artists.FindArtists(term, new ArtistType[] {}, 0, maxResults, false, true);

		}

		[OperationContract]
		public PartialFindResult<SongWithAdditionalNamesContract> FindSongs(string term, int maxResults) {

			return Services.Songs.Find(term, 0, maxResults, false, true, NameMatchMode.Auto, false, null);

		}

		[OperationContract]
		public string[] FindTags(string term, int maxResults) {

			return Services.Tags.FindTags(term);

		}

		[OperationContract]
		public AlbumWithAdditionalNamesContract GetAlbumDetails(string term) {

			var albums = Services.Albums.Find(term, 0, 10, false, false, moveExactToTop: true);
			return albums.Items.FirstOrDefault();

		}

		[OperationContract]
		public AlbumWithAdditionalNamesContract GetAlbumById(int id) {

			var album = Services.Albums.GetAlbumWithAdditionalNames(id);
			return album;

		}

		[OperationContract]
		public ArtistDetailsContract GetArtistDetails(string term) {

			var artists = Services.Artists.FindArtists(term, new ArtistType[] {}, 0, 10, false, false);

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

		[OperationContract]
		public SongWithAdditionalNamesContract GetSongDetails(string term) {

			var songs = Services.Songs.Find(term, 0, 10, false, false,NameMatchMode.Auto, false, null);
			return songs.Items.FirstOrDefault();

		}

		[OperationContract]
		public SongListContract GetSongListById(int id) {

			var list = Services.Songs.GetSongList(id);
			return list;

		}

		[OperationContract]
		public TagContract GetTagByName(string name) {

			var tag = Services.Tags.GetTag(name);
			return tag;

		}

		[OperationContract]
		public UserContract GetUserInfo(string name) {

			var users = Services.Users.FindUsersByName(name);
			return users.FirstOrDefault();

		}

		#endregion

		#region MikuDB-specific queries (TODO: move elsewhere)
		[OperationContract]
		public AlbumWithAdditionalNamesContract GetAlbumByLinkUrl(string url) {
			return Services.Albums.GetAlbumByLink(url);
		}

		[OperationContract]
		public LyricsForSongContract GetRandomSongLyrics(string query) {

			if (string.IsNullOrEmpty(query))
				return Services.Songs.GetRandomSongWithLyricsDetails();
			else
				return Services.Songs.GetRandomLyricsForSong(query);

		}

		[OperationContract]
		public SongWithAdditionalNamesContract GetSongWithPV(PVService service, string pvId) {
			return Services.Songs.GetSongWithPV(service, pvId);
		}

		[OperationContract]
		public UserContract GetUser(string name, string accessKey) {
			return Services.Users.CheckAccessWithKey(name, accessKey);
		}

		#endregion

	}
}
