﻿using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
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

		private IQueryable<Song> AddPVFilter(IQueryable<Song> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<SongName> AddPVFilter(IQueryable<SongName> criteria, bool onlyWithPVs) {

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

		private IQueryable<SongName> AddTimeFilter(IQueryable<SongName> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		private IQueryable<Song> CreateQuery(
			SongQueryParams queryParams, 
			ParsedSongQuery parsedQuery, 
			NameMatchMode? nameMatchMode = null) {
			
			var query = Query<Song>()
				.Where(s => !s.Deleted)
				.WhereHasName(parsedQuery.Name, nameMatchMode ?? queryParams.Common.NameMatchMode)
				.WhereHasArtistParticipationStatus(queryParams.ArtistId, queryParams.ArtistParticipationStatus, queryParams.ChildVoicebanks, id => querySource.Load<Artist>(id))
				.WhereDraftsOnly(queryParams.Common.DraftOnly)
				.WhereStatusIs(queryParams.Common.EntryStatus)
				.WhereHasType(queryParams.SongTypes)
				.WhereHasTag(!string.IsNullOrEmpty(queryParams.Tag) ? queryParams.Tag : parsedQuery.TagName)
				.WhereArtistHasTag(parsedQuery.ArtistTag)
				.WhereArtistHasType(parsedQuery.ArtistType)
				.WhereHasNicoId(parsedQuery.NicoId)
				.WhereIdNotIn(queryParams.IgnoredIds)
				.WhereInUserCollection(queryParams.UserCollectionId)
				.WhereHasLyrics(queryParams.LyricsLanguages);

			query = AddScoreFilter(query, queryParams.MinScore);
			query = AddTimeFilter(query, queryParams.TimeFilter);
			query = AddPVFilter(query, queryParams.OnlyWithPVs);

			return query;

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

		public static Song[] SortByIds(IEnumerable<Song> songs, int[] idList) {
			
			return Model.Helpers.CollectionHelper.SortByIds(songs, idList);

		} 

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		private ParsedSongQuery ParseTextQuery(string query) {
			
			if (string.IsNullOrWhiteSpace(query))
				return new ParsedSongQuery();

			var term = GetTerm(query.Trim(), "tag", "artist-tag", "artist-type");
			
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
					case "artist-type":
						return new ParsedSongQuery { ArtistType = EnumVal<ArtistType>.ParseSafe(term.Value, ArtistType.Unknown) };
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

			var isMoveToTopQuery = 	(queryParams.Common.MoveExactToTop 
				&& queryParams.Common.NameMatchMode != NameMatchMode.StartsWith 
				&& queryParams.Common.NameMatchMode != NameMatchMode.Exact 
				&& queryParams.ArtistId == 0
				&& queryParams.Paging.Start == 0
				&& parsedQuery.HasNameQuery);

			if (isMoveToTopQuery) {
				return GetSongsMoveExactToTop(queryParams, parsedQuery);
			}

			return GetSongs(queryParams, parsedQuery);

		}

		/// <summary>
		/// Get songs, searching by exact matches FIRST.
		/// This mode does not support paging.
		/// </summary>
		private PartialFindResult<Song> GetSongsMoveExactToTop(SongQueryParams queryParams, ParsedSongQuery parsedQuery) {
			
			var sortRule = queryParams.SortRule;
			var maxResults = queryParams.Paging.MaxEntries;
			var getCount = queryParams.Paging.GetTotalCount;

			// Exact query contains the "exact" matches.
			// Note: the matched name does not have to be in user's display language, it can be any name.
			// The songs are sorted by user's display language though
			var exactQ = CreateQuery(queryParams, parsedQuery, NameMatchMode.StartsWith);

			int count;
			int[] ids;
			var exactResults = exactQ
				.OrderBy(sortRule, LanguagePreference)
				.Select(s => s.Id)
				.Take(maxResults)
				.ToArray();

			/*var exactQ = querySource.Query<SongName>()
				.Where(m => !m.Song.Deleted);

			if (queryParams.Common.DraftOnly)
				exactQ = exactQ.Where(a => a.Song.Status == EntryStatus.Draft);

			exactQ = AddScoreFilter(exactQ, queryParams.MinScore);
			exactQ = AddTimeFilter(exactQ, queryParams.TimeFilter);
			exactQ = AddPVFilter(exactQ, queryParams.OnlyWithPVs);

			exactQ = FindHelpers.AddEntryNameFilter(exactQ, parsedQuery.Name, NameMatchMode.StartsWith);

			var songTypes = queryParams.SongTypes;
			if (songTypes.Any())
				exactQ = exactQ.Where(m => songTypes.Contains(m.Song.SongType));
			 
			int count;
			int[] ids;
			var exactResults = exactQ
				.OrderBy(s => s.Value)
				.Select(s => s.Song.Id)
				.ToArray()
				.Distinct()
				//.AddOrder(sortRule, LanguagePreference)
				//.Select(s => s.Id)
				.Take(maxResults)
				.ToArray();
			 			 
			 */

			if (exactResults.Length >= maxResults) {

				ids = exactResults;
				count = getCount ? CreateQuery(queryParams, parsedQuery).Count() : 0;

			} else { 

				var directQ = CreateQuery(queryParams, parsedQuery);

				var direct = directQ
					.OrderBy(sortRule, LanguagePreference)
					.Select(s => s.Id)
					.Take(maxResults)
					.ToArray();

				ids = exactResults
					.Concat(direct)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				count = getCount ? directQ.Count() : 0;

			}

			var songs = SortByIds(
				querySource
					.Query<Song>()
					.Where(s => ids.Contains(s.Id))
					.ToArray(), ids);

			return new PartialFindResult<Song>(songs, count, queryParams.Common.Query, true);

		}

		private PartialFindResult<Song> GetSongs(SongQueryParams queryParams, ParsedSongQuery parsedQuery) {

			var query = CreateQuery(queryParams, parsedQuery);

			var ids = query
				.AddOrder(queryParams.SortRule, LanguagePreference)
				.Select(s => s.Id)
				.Skip(queryParams.Paging.Start)
				.Take(queryParams.Paging.MaxEntries)
				.ToArray();

			var songs = SortByIds(querySource
				.Query<Song>()
				.Where(s => ids.Contains(s.Id))
				.ToArray(), ids);

			var count = (queryParams.Paging.GetTotalCount ? query.Count() : 0);

			return new PartialFindResult<Song>(songs, count, queryParams.Common.Query, false);

		}

	}

}
