using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for albums.
	/// </summary>
	[RoutePrefix("api/albums")]
	public class AlbumApiController : ApiController {

		private const int maxResults = 10;
		private readonly AlbumQueries queries;
		private readonly AlbumService service;

		public AlbumApiController(AlbumQueries queries, AlbumService service) {			
			this.queries = queries;
			this.service = service;
		}

		/// <summary>
		/// Gets an album by Id.
		/// </summary>
		/// <param name="id">Album Id (required).</param>
		/// <param name="fields">Optional fields (optional). Possible values are artists, names, pvs, tags, webLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <example>http://vocadb.net/api/albums/1</example>
		/// <returns>Album data.</returns>
		[Route("{id:int}")]
		public AlbumForApiContract GetOne(int id, AlbumOptionalFields fields = AlbumOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default) {
			
			var album = service.GetAlbumWithMergeRecord(id, (a, m) => 
				new AlbumForApiContract(a, m, lang, 
					fields.HasFlag(AlbumOptionalFields.Artists), 
					fields.HasFlag(AlbumOptionalFields.Names), 
					fields.HasFlag(AlbumOptionalFields.PVs), 
					fields.HasFlag(AlbumOptionalFields.Tags), 
					fields.HasFlag(AlbumOptionalFields.WebLinks)));

			return album;

		}

		/*
		[Route("")]
		public PartialFindResult<AlbumForApiContract> GetList(string query, DiscType discType = DiscType.Unknown,
			int start = 0, bool getTotalCount = false, AlbumSortRule sort = AlbumSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact, 
			AlbumOptionalFields fields = AlbumOptionalFields.None, 
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var queryParams = new AlbumQueryParams(query, discType, start, maxResults, false, getTotalCount, nameMatchMode, sort);

			var entries = service.Find(a => 
				new AlbumForApiContract(a, null, lang, 
					fields.HasFlag(AlbumOptionalFields.Artists), 
					fields.HasFlag(AlbumOptionalFields.Names), 
					fields.HasFlag(AlbumOptionalFields.PVs), 
					fields.HasFlag(AlbumOptionalFields.Tags), 
					fields.HasFlag(AlbumOptionalFields.WebLinks)), 
				queryParams);
			
			return entries;

		}*/

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

	[Flags]
	public enum AlbumOptionalFields {

		None = 0,
		Artists = 1,
		Names = 2,
		PVs = 4,
		Tags = 8,
		WebLinks = 16

	}

}