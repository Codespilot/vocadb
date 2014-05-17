﻿using System;
using System.Linq;
using System.ServiceModel;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Services {

	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "QueryService" in code, svc and config file together.
	[ServiceContract(Namespace = Schemas.VocaDb)]
	public class QueryService {

		private readonly TagQueries tagQueries;
		private readonly IUserPermissionContext userPermissionContext;
		private ServiceModel Services { get; set; }

		public QueryService(ServiceModel services, TagQueries tagQueries, IUserPermissionContext userPermissionContext) {
			Services = services;
			this.tagQueries = tagQueries;
			this.userPermissionContext = userPermissionContext;
		}

		#region Common queries
		[OperationContract]
		public PartialFindResult<AlbumContract> FindAlbums(string term, int maxResults, 
			NameMatchMode nameMatchMode = NameMatchMode.Auto, AlbumSortRule sort = AlbumSortRule.NameThenReleaseDate) {

			return Services.Albums.Find(term, DiscType.Unknown, 0, maxResults, false, true, moveExactToTop: true, nameMatchMode: nameMatchMode, sortRule: sort);

		}

		[OperationContract]
		public PartialFindResult<AlbumWithAdditionalNamesContract> FindAlbumsAdvanced(string term, int maxResults) {

			return Services.Albums.FindAdvanced(term, new PagingProperties(0, maxResults, true), AlbumSortRule.Name);

		}

		[OperationContract]
		public AllEntriesSearchResult FindAll(string term, int maxResults) {

			return Services.Other.Find(term, maxResults, true);

		}

		[OperationContract]
		public PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(string term, int maxResults, NameMatchMode nameMatchMode = NameMatchMode.Auto) {

			return Services.Artists.FindArtists(new ArtistQueryParams(term, new ArtistType[] {}, 0, maxResults, false, true, nameMatchMode, ArtistSortRule.Name, true));

		}

		[OperationContract]
		public PartialFindResult<SongWithAlbumAndPVsContract> FindSongs(string term, int maxResults, NameMatchMode nameMatchMode = NameMatchMode.Auto) {

			var sampleSize = Math.Min(maxResults * 2, 30);

			var results = Services.Songs.FindWithAlbum(new SongQueryParams(
				term, new SongType[] {}, 0, sampleSize, false, true, nameMatchMode, SongSortRule.Name, false, true, null), false);

			return new PartialFindResult<SongWithAlbumAndPVsContract>(results.Items.Take(maxResults).ToArray(), results.TotalCount, results.Term, results.FoundExactMatch);

		}

		[OperationContract]
		public string[] FindTags(string term, int maxResults) {

			return tagQueries.FindNames(term, true, false, 10);

		}

		[OperationContract]
		public AlbumContract GetAlbumDetails(string term, AlbumSortRule sort = AlbumSortRule.NameThenReleaseDate) {

			var matchMode = NameMatchMode.Auto;
			term = FindHelpers.GetMatchModeAndQueryForSearch(term, ref matchMode);

			var albums = Services.Albums.Find(term, DiscType.Unknown, 0, 10, false, false, moveExactToTop: true, sortRule: sort, nameMatchMode: matchMode);
			return albums.Items.FirstOrDefault();

		}

		[OperationContract]
		public AlbumDetailsContract GetAlbumById(int id) {

			var album = Services.Albums.GetAlbumDetails(id, null);
			return album;

		}

		[OperationContract]
		public ArtistDetailsContract GetArtistDetails(string term) {

			var matchMode = NameMatchMode.Auto;
			term = FindHelpers.GetMatchModeAndQueryForSearch(term, ref matchMode);

			var artists = Services.Artists.FindArtists(new ArtistQueryParams(term, new ArtistType[] {}, 0, 10, 
				false, false, matchMode, ArtistSortRule.Name, true));

			if (!artists.Items.Any())
				return null;

			return Services.Artists.GetArtistDetails(artists.Items[0].Id);

		}

		[OperationContract]
		public ArtistDetailsContract GetArtistById(int id) {

			var artist = Services.Artists.GetArtistDetails(id);
			return artist;

		}

		[OperationContract]
		public ArtistForApiContract[] GetArtistsWithYoutubeChannels(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default) {

			return Services.Artists.GetArtistsWithYoutubeChannels(languagePreference);

		}

		[OperationContract]
		public SongDetailsContract GetSongById(int id) {

			var song = Services.Songs.GetSongDetails(id, 0, null);
			return song;

		}

		[OperationContract]
		public SongDetailsContract GetSongDetailsByNameArtistAndAlbum(string name, string artist, string album) {

			return Services.Songs.XGetSongByNameArtistAndAlbum(name, artist, album);

		}

		[OperationContract]
		public SongDetailsContract GetSongDetails(string term, ContentLanguagePreference? language = null) {

			if (language.HasValue)
				userPermissionContext.OverrideLanguage(language.Value);

			var matchMode = NameMatchMode.Auto;
			term = FindHelpers.GetMatchModeAndQueryForSearch(term, ref matchMode);

			var song = Services.Songs.FindFirstDetails(term, matchMode);
			return song;

		}

		[OperationContract]
		public SongListContract GetSongListById(int id) {

			var list = Services.Songs.GetSongList(id);
			return list;

		}

		[OperationContract]
		public TagContract GetTagByName(string name) {

			var tag = Services.Tags.FindTag(name);
			return tag;

		}

		[OperationContract]
		public UserContract GetUserInfo(string name) {

			var users = Services.Users.FindUsersByName(name, NameMatchMode.Exact);
			return users.FirstOrDefault();

		}

		#endregion

		#region MikuDB-specific queries (TODO: move elsewhere)
		[OperationContract]
		public AlbumContract GetAlbumByLinkUrl(string url) {
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
		public SongContract GetSongWithPV(PVService service, string pvId) {
			return Services.Songs.GetSongWithPV(service, pvId);
		}

		[OperationContract]
		public UserContract GetUser(string name, string accessKey) {
			return Services.Users.CheckAccessWithKey(name, accessKey, "localhost");
		}

		#endregion

	}
}
