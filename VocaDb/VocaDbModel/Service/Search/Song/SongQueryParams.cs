using System;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.Song {

	/// <summary>
	/// Query parameters for songs
	/// </summary>
	public class SongQueryParams {

		public SongQueryParams() {

			Common = new CommonSearchParams();
			Paging = new PagingProperties(0, 30, true);
			SongTypes = new SongType[] {};

		}

		/// <param name="query">Query search string. Can be null or empty, in which case no filtering by name is done.</param>
		/// <param name="songTypes">Allowed song types. Can be null or empy, in which case no filtering by song type is done.</param>
		/// <param name="start">0-based order number of the first item to be returned.</param>
		/// <param name="maxResults">Maximum number of results to be returned.</param>
		/// <param name="draftsOnly">Whether to return only entries with a draft status.</param>
		/// <param name="getTotalCount">Whether to return the total number of entries matching the criteria.</param>
		/// <param name="nameMatchMode">Mode for name maching. Ignored when query string is null or empty.</param>
		/// <param name="sortRule">Sort rule for results.</param>
		/// <param name="onlyByName">Whether to search items only by name, and not for example NicoId. Ignored when query string is null or empty.</param>
		/// <param name="moveExactToTop">Whether to move exact match to the top of search results.</param>
		/// <param name="ignoredIds">List of entries to be ignored. Can be null in which case no filtering is done.</param>
		public SongQueryParams(string query, SongType[] songTypes, int start, int maxResults,
			bool draftsOnly, bool getTotalCount, NameMatchMode nameMatchMode, SongSortRule sortRule, 
			bool onlyByName, bool moveExactToTop, int[] ignoredIds) {

			Common = new CommonSearchParams(query, draftsOnly, nameMatchMode, onlyByName, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			SongTypes = songTypes ?? new SongType[] {};
			SortRule = sortRule;
			IgnoredIds = ignoredIds ?? new int[] {};
			TimeFilter = TimeSpan.Zero;
			OnlyWithPVs = false;

		}

		public CommonSearchParams Common { get; set; }

		public int[] IgnoredIds { get; set; }

		public bool OnlyWithPVs { get; set; }

		public PagingProperties Paging { get; set; }

		public SongType[] SongTypes { get; set; }

		public SongSortRule SortRule { get; set; }

		public TimeSpan TimeFilter { get; set; }

	}

}