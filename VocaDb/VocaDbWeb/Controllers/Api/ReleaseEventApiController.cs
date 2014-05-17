using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for album release events.
	/// </summary>
	[RoutePrefix("api/releaseEvents")]
	public class ReleaseEventApiController : ApiController {

		private const int defaultMax = 10;
		private readonly IEventRepository repository;

		public ReleaseEventApiController(IEventRepository repository) {
			this.repository = repository;
		}

		/// <summary>
		/// Gets a page of release events.
		/// </summary>
		/// <param name="query">Event name query (optional).</param>
		/// <param name="seriesId">Filter by series Id.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">
		/// Sort rule (optional, defaults to Name). 
		/// Possible values are None, Name, Date, SeriesName.
		/// </param>
		/// <param name="fields">
		/// Optional fields (optional). Possible values are Description, Series.
		/// </param>
		/// <returns>Page of events.</returns>
		/// <example>http://vocadb.net/api/releaseEvents?query=Voc@loid</example>
		[Route("")]
		public PartialFindResult<ReleaseEventForApiContract> GetList(
			string query = "", 
			int seriesId = 0,
			int start = 0, 
			int maxResults = defaultMax,
			bool getTotalCount = false, 
			EventSortRule sort = EventSortRule.Name,
			ReleaseEventOptionalFields fields = ReleaseEventOptionalFields.None
			) {
			
			query = !string.IsNullOrEmpty(query) ? FindHelpers.CleanTerm(query.Trim()) : string.Empty;		

			return repository.HandleQuery(ctx => {
				
				var q = ctx.Query()
					.WhereHasName(query)
					.WhereHasSeries(seriesId);

				var entries = q
					.OrderBy(sort)
					.Skip(start)
					.Take(maxResults)
					.ToArray()
					.Select(e => new ReleaseEventForApiContract(e, fields))
					.ToArray();

				var count = 0;

				if (getTotalCount) {
					
					count = q.Count();

				}

				return new PartialFindResult<ReleaseEventForApiContract>(entries, count);

			});

		}

	}

}