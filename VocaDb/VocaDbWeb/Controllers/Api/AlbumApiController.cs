using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for albums.
	/// </summary>
	[RoutePrefix("api/albums")]
	public class AlbumApiController : ApiController {

		private const int absoluteMax = 30;
		private const int defaultMax = 10;
		private readonly IEntryThumbPersister thumbPersister;
		private readonly AlbumQueries queries;
		private readonly AlbumService service;

		public AlbumApiController(AlbumQueries queries, AlbumService service, IEntryThumbPersister thumbPersister) {			
			this.queries = queries;
			this.service = service;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Gets an album by Id.
		/// </summary>
		/// <param name="id">Album Id (required).</param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are artists, names, pvs, tags, webLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/albums/1</example>
		/// <returns>Album data.</returns>
		[Route("{id:int}")]
		public AlbumForApiContract GetOne(int id, AlbumOptionalFields fields = AlbumOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var ssl = WebHelper.IsSSL(Request);
			var album = service.GetAlbumWithMergeRecord(id, (a, m) => new AlbumForApiContract(a, m, lang, thumbPersister, ssl, fields));

			return album;

		}

		/// <summary>
		/// Gets a page of albums.
		/// </summary>
		/// <param name="query">Album name query (optional).</param>
		/// <param name="discTypes">
		/// Disc type. By default nothing. Possible values are Album, Single, EP, SplitAlbum, Compilation, Video, Other. Note: only one type supported for now.
		/// </param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">
		/// Sort rule (optional, defaults to Name). 
		/// Possible values are None, Name, ReleaseDate, ReleaseDateWithNulls, AdditionDate, RatingAverage, RatingTotal, NameThenReleaseDate.
		/// </param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Exact).</param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are artists, names, pvs, tags, webLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of albums.</returns>
		/// <example>http://vocadb.net/api/albums?query=Synthesis&amp;discTypes=Album</example>
		[Route("")]
		public PartialFindResult<AlbumForApiContract> GetList(
			string query = "", 
			DiscType discTypes = DiscType.Unknown,
			string tag = null,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			AlbumSortRule sort = AlbumSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var queryParams = new AlbumQueryParams(query, discTypes, start, Math.Min(maxResults, absoluteMax), false, getTotalCount, nameMatchMode, sort) {
				Tag = tag
			};
			var ssl = WebHelper.IsSSL(Request);

			var entries = service.Find(a => new AlbumForApiContract(a, null, lang, thumbPersister, ssl, fields), queryParams);
			
			return entries;

		}

		/// <summary>
		/// Gets tracks for an album.
		/// </summary>
		/// <param name="id">Album ID (required).</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>List of tracks for the album.</returns>
		/// <example>http://vocadb.net/api/albums/1/tracks</example>
		[Route("{id:int}/tracks")]
		public SongInAlbumContract[] GetTracks(int id, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var tracks = service.GetAlbum(id, a => a.Songs.Select(s => new SongInAlbumContract(s, lang, false)).ToArray());

			return tracks;

		}

		/// <summary>
		/// Gets a complete list of album versions and Ids.
		/// Intended for integration to other systems.
		/// </summary>
		/// <returns>List of album IDs with versions.</returns>
		[Route("versions")]
		[ApiExplorerSettings(IgnoreApi=true)]
		public EntryIdAndVersionContract[] GetVersions() {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(a => new { a.Id, a.Version })
					.ToArray()
					.Select(v => new EntryIdAndVersionContract(v.Id, v.Version))
					.ToArray());

			return versions;

		}

	}

}