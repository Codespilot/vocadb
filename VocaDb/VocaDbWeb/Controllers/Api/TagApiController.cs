using System.Web.Http;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {
	
	/// <summary>
	/// API queries for tags.
	/// </summary>
	[RoutePrefix("api/tags")]
	public class TagApiController : ApiController {

		private readonly TagQueries queries;

		public TagApiController(TagQueries queries) {
			this.queries = queries;
		}

		/// <summary>
		/// Find tag names by a part of name.
		/// 
		/// Matching is done anywhere from the name.
		/// Spaces are automatically converted into underscores.
		/// </summary>
		/// <param name="query">Tag name query, for example "rock".</param>
		/// <param name="allowAliases">
		/// Whether to find tags that are aliases of other tags as well. 
		/// If false, only tags that are not aliases will be listed.
		/// </param>
		/// <returns>
		/// List of tag names, for example "vocarock", matching the query. Cannot be null.
		/// </returns>
		[Route("names")]
		public string[] GetNames(string query, bool allowAliases = true) {
			
			return queries.FindNames(query, allowAliases);

		}

	}

}