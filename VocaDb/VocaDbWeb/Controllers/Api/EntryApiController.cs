using System;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// Controller for managing base class for common entries.
	/// </summary>
	[RoutePrefix("api/entries")]
	public class EntryApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;

		private readonly IEntryThumbPersister entryThumbPersister;
		private readonly IUserPermissionContext permissionContext;
		private readonly IAlbumRepository repository;

		private int GetMaxResults(int max) {
			return Math.Min(max, absoluteMax);	
		}

		public EntryApiController(IAlbumRepository repository, IUserPermissionContext permissionContext, IEntryThumbPersister entryThumbPersister) {
			this.repository = repository;
			this.permissionContext = permissionContext;
			this.entryThumbPersister = entryThumbPersister;
		}

		/// <summary>
		/// Find entries.
		/// </summary>
		/// <param name="query">Entry name query (optional).</param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="nameMatchMode">Match mode for entry name (optional, defaults to Exact).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, MainPicture, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of entries.</returns>
		/// <example>http://vocadb.net/api/entries?query=164&amp;fields=MainPicture</example>
		[Route("")]
		public PartialFindResult<EntryForApiContract> GetList(
			string query, 
			string tag = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			EntryOptionalFields fields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			var ssl = WebHelper.IsSSL(Request);
			maxResults = GetMaxResults(maxResults);
			query = FindHelpers.GetMatchModeAndQueryForSearch(query, ref nameMatchMode);
			var canonized = ArtistHelper.GetCanonizedName(query);

			return repository.HandleQuery(ctx => {

				// Get all applicable names per entry type
				var artistNames = ctx.OfType<Artist>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(canonized, nameMatchMode)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(ArtistSortRule.Name, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Artist)
					.ToArray();

				var albumNames = ctx.OfType<Album>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(query, nameMatchMode)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(AlbumSortRule.Name, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Album)
					.ToArray();

				var songNames = ctx.OfType<Song>().Query()
					.Where(a => !a.Deleted)
					.WhereHasName(query, nameMatchMode)
					.WhereHasTag(tag)
					.WhereStatusIs(status)
					.OrderBy(SongSortRule.Name, lang)
					.Take(start + maxResults)
					.SelectEntryBase(lang, EntryType.Song)
					.ToArray();

				// Get page of combined names
				var entryNames = artistNames.Concat(albumNames).Concat(songNames)
					.OrderBy(e => e.DefaultName)
					.Skip(start)
					.Take(maxResults)
					.ToArray();
				
				var artistIds = entryNames.Where(e => e.EntryType == EntryType.Artist).Select(a => a.Id).ToArray();
				var albumIds = entryNames.Where(e => e.EntryType == EntryType.Album).Select(a => a.Id).ToArray();
				var songIds = entryNames.Where(e => e.EntryType == EntryType.Song).Select(a => a.Id).ToArray();
				var allIds = entryNames.Select(e => e.Id).ToArray();

				// Get the actual entries in the page
				var artists = artistIds.Any() ? ctx.OfType<Artist>().Query()
					.Where(a => artistIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, entryThumbPersister, ssl, fields)) : new EntryForApiContract[0];

				var albums = albumIds.Any() ? ctx.OfType<Album>().Query()
					.Where(a => albumIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, entryThumbPersister, ssl, fields)) : new EntryForApiContract[0];

				var songs = songIds.Any() ? ctx.OfType<Song>().Query()
					.Where(a => songIds.Contains(a.Id))
					.ToArray()
					.Select(a => new EntryForApiContract(a, lang, fields)) : new EntryForApiContract[0];

				// Merge and sort the final list
				var entries = CollectionHelper.SortByIds(artists.Concat(albums).Concat(songs), allIds);

				var count = 0;

				if (getTotalCount) {
					
					count = 
						ctx.OfType<Artist>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName(canonized, nameMatchMode)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count() + 
						ctx.OfType<Album>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName(query, nameMatchMode)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count() +
						ctx.OfType<Song>().Query()
							.Where(a => !a.Deleted)
							.WhereHasName(query, nameMatchMode)
							.WhereHasTag(tag)
							.WhereStatusIs(status)
							.Count();


				}

				return new PartialFindResult<EntryForApiContract>(entries.ToArray(), count);

			});

		}

	}

}