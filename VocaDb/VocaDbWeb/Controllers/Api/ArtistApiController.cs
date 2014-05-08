using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for artists.
	/// </summary>
	[RoutePrefix("api/artists")]
	public class ArtistApiController : ApiController {

		private const int absoluteMax = 30;
		private const int defaultMax = 10;
		private readonly ArtistQueries queries;
		private readonly ArtistService service;
		private readonly IEntryThumbPersister thumbPersister;

		public ArtistApiController(ArtistQueries queries, ArtistService service, IEntryThumbPersister thumbPersister) {
			this.queries = queries;
			this.service = service;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Gets an artist by Id.
		/// </summary>
		/// <param name="id">Artist ID (required).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Artist data.</returns>
		/// <example>http://vocadb.net/api/artists/1</example>
		[Route("{id:int}")]
		public ArtistForApiContract GetOne(int id,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			var artist = service.GetArtistWithMergeRecord(id, (a, m) => new ArtistForApiContract(a, lang, 
				thumbPersister, WebHelper.IsSSL(Request), fields));

			return artist;

		}

		/// <summary>
		/// Find artists.
		/// </summary>
		/// <param name="query">Artist name query (optional).</param>
		/// <param name="artistTypes">Filtered artist type (optional).</param>
		/// <param name="tag">Filter by tag (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate, AdditionDateAsc.</param>
		/// <param name="nameMatchMode">Match mode for artist name (optional, defaults to Exact).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, Groups, Members, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of artists.</returns>
		/// <example>http://vocadb.net/api/artists?query=Tripshots&amp;artistTypes=Producer</example>
		[Route("")]
		public PartialFindResult<ArtistForApiContract> GetList(
			string query = "", 
			ArtistTypes artistTypes = ArtistTypes.Nothing,
			string tag = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			ArtistSortRule sort = ArtistSortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			ArtistOptionalFields fields = ArtistOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			query = FindHelpers.GetMatchModeAndQueryForSearch(query, ref nameMatchMode);
			var types = ArtistHelper.GetArtistTypesFromFlags(artistTypes);

			var param = new ArtistQueryParams(query, types, start, Math.Min(maxResults, absoluteMax), false, getTotalCount, nameMatchMode, sort, false) {
				Tag = tag
			};

			var ssl = WebHelper.IsSSL(Request);
			var artists = service.FindArtists(s => new ArtistForApiContract(s, lang, thumbPersister, ssl, fields), param);

			return artists;

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

}