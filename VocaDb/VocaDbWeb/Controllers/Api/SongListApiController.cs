using System;
using System.Web.Http;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for song lists.
	/// </summary>
	[RoutePrefix("api/songLists")]
	public class SongListApiController : ApiController { 

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly SongListQueries queries;

		public SongListApiController(SongListQueries queries) {
			this.queries = queries;
		}

		/// <summary>
		/// Gets a list of songs in a song list.
		/// </summary>
		/// <param name="listId">ID of the song list.</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 50).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Albums, Artists, Names, PVs, Tags, ThumbUrl, WebLinks.
		/// </param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of songs.</returns>
		[Route("{listId:int}/songs")]
		public PartialFindResult<SongInListForApiContract> GetSongs(int listId, 
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			SongOptionalFields fields = SongOptionalFields.None, ContentLanguagePreference lang = ContentLanguagePreference.Default
			) {
			
			maxResults = Math.Min(maxResults, absoluteMax);

			return queries.GetSongsInList(listId, start, maxResults, getTotalCount, songInList => 
				new SongInListForApiContract(songInList, lang, fields));

		}

	}

}