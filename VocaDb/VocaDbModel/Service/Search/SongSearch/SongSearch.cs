using System;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Search.SongSearch {

	public class SongSearch {

		private readonly ContentLanguagePreference languagePreference;
		private readonly IQuerySource querySource;

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private static IQueryable<Song> AddNameFilter(IQueryable<Song> directQ, string query, NameMatchMode nameMatchMode, bool onlyByName) {

			var matchMode = FindHelpers.GetMatchMode(query, nameMatchMode);

			if (matchMode == NameMatchMode.Exact) {

				return directQ.Where(s =>
					s.Names.SortNames.English == query
						|| s.Names.SortNames.Romaji == query
						|| s.Names.SortNames.Japanese == query);

			} else if (matchMode == NameMatchMode.StartsWith) {

				return directQ.Where(s =>
					s.Names.SortNames.English.StartsWith(query)
						|| s.Names.SortNames.Romaji.StartsWith(query)
						|| s.Names.SortNames.Japanese.StartsWith(query));

			} else {

				return directQ.Where(s =>
					s.Names.SortNames.English.Contains(query)
						|| s.Names.SortNames.Romaji.Contains(query)
						|| s.Names.SortNames.Japanese.Contains(query)
						|| (!onlyByName && s.NicoId != null && s.NicoId == query));

			}

		}

		private IQueryable<Song> AddPVFilter(IQueryable<Song> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<ArtistForSong> AddPVFilter(IQueryable<ArtistForSong> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.Song.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<SongName> AddPVFilter(IQueryable<SongName> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.Song.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<SongTagUsage> AddPVFilter(IQueryable<SongTagUsage> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.Song.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<Song> AddScoreFilter(IQueryable<Song> query, int minScore) {

			if (minScore <= 0)
				return query;

			return query.Where(q => q.RatingScore >= minScore);

		}

		private IQueryable<SongName> AddScoreFilter(IQueryable<SongName> query, int minScore) {

			if (minScore <= 0)
				return query;

			return query.Where(q => q.Song.RatingScore >= minScore);

		} 

		private IQueryable<Song> AddTimeFilter(IQueryable<Song> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.CreateDate >= since);

		}

		private IQueryable<ArtistForSong> AddTimeFilter(IQueryable<ArtistForSong> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		private IQueryable<SongName> AddTimeFilter(IQueryable<SongName> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		private IQueryable<SongTagUsage> AddTimeFilter(IQueryable<SongTagUsage> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		private SearchWord GetTerm(string query, params string[] testTerms) {
			//var match = Regex.Match(query, @"^(\w+)\s?:\s?(.+)"); // Search for term at the start of query

			/*if (match.Success)
				return new SearchWord(match.Groups[1].Value.ToLowerInvariant(), match.Groups[2].Value);
			else
				return null;*/

			return (
				from term in testTerms 
				where query.StartsWith(term + ":", StringComparison.InvariantCultureIgnoreCase) 
				select new SearchWord(term, query.Substring(term.Length + 1).TrimStart()))
			.FirstOrDefault();

		}

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		private ParsedSongQuery ParseTextQuery(string query) {
			
			if (string.IsNullOrWhiteSpace(query))
				return new ParsedSongQuery();

			var term = GetTerm(query.Trim(), "tag", "artist-tag");
			
			if (term == null) {

				var nicoId = VideoService.NicoNicoDouga.GetIdByUrl(query);

				if (!string.IsNullOrEmpty(nicoId))
					return new ParsedSongQuery { NicoId = nicoId };


			} else {

				switch (term.PropertyName) {
					case "tag":
						return new ParsedSongQuery { TagName = term.Value };
					case "artist-tag":
						return new ParsedSongQuery { ArtistTag = term.Value };
				}
				
			}

			return new ParsedSongQuery { Name = query.Trim() };

		}

		public SongSearch(IQuerySource querySource, ContentLanguagePreference languagePreference) {
			this.querySource = querySource;
			this.languagePreference = languagePreference;
		}

		/// <summary>
		/// Finds songs based on criteria.
		/// </summary>
		/// <param name="queryParams">Query parameters. Cannot be null.</param>
		/// <returns>List of song search results. Cannot be null.</returns>
		public PartialFindResult<Song> Find(SongQueryParams queryParams) {

			ParamIs.NotNull(() => queryParams);

			var query = queryParams.Common.Query ?? string.Empty;
			var parsedQuery = ParseTextQuery(query);

			Song[] songs = GetSongs(queryParams, parsedQuery);

			int count = (queryParams.Paging.GetTotalCount ? GetSongCount(queryParams, parsedQuery) : 0);

			return new PartialFindResult<Song>(songs, count, queryParams.Common.Query, false);

		}

		private Song[] GetSongs(SongQueryParams queryParams, ParsedSongQuery parsedQuery) {

			var ignoreIds = queryParams.IgnoredIds;
			var nameMatchMode = queryParams.Common.NameMatchMode;
			var songTypes = queryParams.SongTypes;
			var sortRule = queryParams.SortRule;
			var start = queryParams.Paging.Start;
			var maxResults = queryParams.Paging.MaxEntries;

			bool filterByType = songTypes.Any();
			Song[] songs;

			int[] exactResults;

			if (queryParams.Common.MoveExactToTop 
				&& nameMatchMode != NameMatchMode.StartsWith 
				&& nameMatchMode != NameMatchMode.Exact 
				&& queryParams.ArtistId == 0
				&& queryParams.Paging.Start == 0
				&& parsedQuery.HasNameQuery) {

				var exactQ = querySource.Query<SongName>()
					.Where(m => !m.Song.Deleted);

				if (queryParams.Common.DraftOnly)
					exactQ = exactQ.Where(a => a.Song.Status == EntryStatus.Draft);

				exactQ = AddScoreFilter(exactQ, queryParams.MinScore);
				exactQ = AddTimeFilter(exactQ, queryParams.TimeFilter);
				exactQ = AddPVFilter(exactQ, queryParams.OnlyWithPVs);

				exactQ = FindHelpers.AddEntryNameFilter(exactQ, parsedQuery.Name, NameMatchMode.StartsWith);

				if (filterByType)
					exactQ = exactQ.Where(m => songTypes.Contains(m.Song.SongType));

				exactResults = exactQ
					.Select(m => m.Song)
					.AddOrder(sortRule, LanguagePreference)
					.Select(s => s.Id)
					.Take(maxResults)
					.ToArray()
					.Distinct()
					.ToArray();

				if (queryParams.Paging.Start == 0 && exactResults.Length >= maxResults)
					return querySource.Query<Song>()
						.Where(s => exactResults.Contains(s.Id))
						.AddOrder(sortRule, LanguagePreference)
						.ToArray();

			} else {
				exactResults = new int[0];
			}

			var directQ = Query<Song>()
				.Where(s => !s.Deleted)
				.WhereHasName(parsedQuery.Name, nameMatchMode)
				.WhereHasArtist(queryParams.ArtistId)
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereHasType(queryParams.SongTypes)
				.WhereHasTag(parsedQuery.TagName)
				.WhereArtistHasTag(parsedQuery.ArtistTag)
				.WhereHasNicoId(parsedQuery.NicoId);

			directQ = AddScoreFilter(directQ, queryParams.MinScore);
			directQ = AddTimeFilter(directQ, queryParams.TimeFilter);
			directQ = AddPVFilter(directQ, queryParams.OnlyWithPVs);

			var direct = directQ
				.AddOrder(sortRule, LanguagePreference)
				.Select(s => s.Id)
				.Skip(start)
				.Take(maxResults)
				.ToArray();

			var page = exactResults.Concat(direct)
				.Distinct()
				.Where(e => !ignoreIds.Contains(e))
				.Take(maxResults)
				.ToArray();

			songs = querySource
				.Query<Song>()
				.Where(s => page.Contains(s.Id))
				.AddOrder(sortRule, LanguagePreference) // TODO: should actually sort by the original Id order
				.ToArray();

			return songs;

		}

		private int GetSongCount(SongQueryParams queryParams, ParsedSongQuery parsedQuery) {

			ParamIs.NotNull(() => queryParams);

			var directQ = Query<Song>()
				.Where(s => !s.Deleted)
				.WhereHasName(parsedQuery.Name, queryParams.Common.NameMatchMode)
				.WhereHasArtist(queryParams.ArtistId)
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereHasType(queryParams.SongTypes)
				.WhereHasTag(parsedQuery.TagName)
				.WhereArtistHasTag(parsedQuery.ArtistTag)
				.WhereHasNicoId(parsedQuery.NicoId);

			directQ = AddScoreFilter(directQ, queryParams.MinScore);
			directQ = AddTimeFilter(directQ, queryParams.TimeFilter);
			directQ = AddPVFilter(directQ, queryParams.OnlyWithPVs);

			return directQ.Count();

		}

	}

}
