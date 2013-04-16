using System;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;

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
						|| (!onlyByName &&
							(s.ArtistString.Japanese.Contains(query)
								|| s.ArtistString.Romaji.Contains(query)
								|| s.ArtistString.English.Contains(query)))
						|| (s.NicoId != null && s.NicoId == query));

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

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
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

			Song[] songs = GetSongs(queryParams);

			int count = (queryParams.Paging.GetTotalCount ? GetSongCount(queryParams) : 0);

			return new PartialFindResult<Song>(songs, count, queryParams.Common.Query, false);

		}

		private Song[] GetSongs(SongQueryParams queryParams) {

			var draftsOnly = queryParams.Common.DraftOnly;
			var ignoreIds = queryParams.IgnoredIds;
			var nameMatchMode = queryParams.Common.NameMatchMode;
			var onlyByName = queryParams.Common.OnlyByName;
			var query = queryParams.Common.Query;
			var songTypes = queryParams.SongTypes;
			var sortRule = queryParams.SortRule;
			var start = queryParams.Paging.Start;
			var maxResults = queryParams.Paging.MaxEntries;

			bool filterByType = songTypes.Any();
			Song[] songs;

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var q = Query<Song>()
					.Where(s => !s.Deleted
						&& !ignoreIds.Contains(s.Id));

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.SongType));

				q = AddTimeFilter(q, queryParams.TimeFilter);
				q = AddPVFilter(q, queryParams.OnlyWithPVs);

				q = q.AddOrder(sortRule, LanguagePreference);

				songs = q
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else if (queryParams.ArtistId != 0) {

				int artistId = queryParams.ArtistId;

				var q = Query<ArtistForSong>()
					.Where(m => !m.Song.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					q = q.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.Song.SongType));

				q = AddTimeFilter(q, queryParams.TimeFilter);
				q = AddPVFilter(q, queryParams.OnlyWithPVs);

				songs = q
					.Select(m => m.Song)
					.AddOrder(sortRule, LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else {

				query = query.Trim();

				Song[] exactResults;

				if (queryParams.Common.MoveExactToTop && nameMatchMode != NameMatchMode.StartsWith && nameMatchMode != NameMatchMode.Exact) {

					var exactQ = querySource.Query<SongName>()
						.Where(m => !m.Song.Deleted);

					if (draftsOnly)
						exactQ = exactQ.Where(a => a.Song.Status == EntryStatus.Draft);

					exactQ = AddTimeFilter(exactQ, queryParams.TimeFilter);
					exactQ = AddPVFilter(exactQ, queryParams.OnlyWithPVs);

					exactQ = FindHelpers.AddEntryNameFilter(exactQ, query, NameMatchMode.StartsWith);

					if (filterByType)
						exactQ = exactQ.Where(m => songTypes.Contains(m.Song.SongType));

					exactResults = exactQ
						.Select(m => m.Song)
						.AddOrder(sortRule, LanguagePreference)
						.Distinct()
						.Take(maxResults)
						.ToArray();

					if (queryParams.Paging.Start == 0 && exactResults.Length >= maxResults)
						return exactResults;

				} else {
					exactResults = new Song[] { };
				}

				// Searching by SortNames can be disabled in the future because all names should be included in the Names list anyway.
				var directQ = Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					directQ = directQ.Where(s => songTypes.Contains(s.SongType));

				directQ = AddTimeFilter(directQ, queryParams.TimeFilter);
				directQ = AddPVFilter(directQ, queryParams.OnlyWithPVs);

				directQ = AddNameFilter(directQ, query, nameMatchMode, onlyByName);

				directQ = directQ.AddOrder(sortRule, LanguagePreference);

				var direct = directQ.ToArray();

				var additionalNamesQ = Query<SongName>()
					.Where(m => !m.Song.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				additionalNamesQ = AddTimeFilter(additionalNamesQ, queryParams.TimeFilter);
				additionalNamesQ = AddPVFilter(additionalNamesQ, queryParams.OnlyWithPVs);

				additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

				if (filterByType)
					additionalNamesQ = additionalNamesQ.Where(m => songTypes.Contains(m.Song.SongType));

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.AddOrder(sortRule, LanguagePreference)
					.Distinct()
					.ToArray();

				var entries = exactResults.Union(direct.Union(additionalNames))
					.Where(e => !ignoreIds.Contains(e.Id))
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				songs = entries;

			}

			return songs;

		}

		private int GetSongCount(SongQueryParams queryParams) {

			ParamIs.NotNull(() => queryParams);

			var draftsOnly = queryParams.Common.DraftOnly;
			var nameMatchMode = queryParams.Common.NameMatchMode;
			var onlyByName = queryParams.Common.OnlyByName;
			var query = queryParams.Common.Query;
			var songTypes = queryParams.SongTypes;
			var timeFilter = queryParams.TimeFilter;
			var onlyWithPVs = queryParams.OnlyWithPVs;
			var filterByType = songTypes.Any();

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var q = Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.SongType));

				q = AddTimeFilter(q, timeFilter);
				q = AddPVFilter(q, onlyWithPVs);

				return q.Count();

			} else if (queryParams.ArtistId != 0) {

				int artistId = queryParams.ArtistId;

				var q = Query<ArtistForSong>()
					.Where(m => !m.Song.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					q = q.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.Song.SongType));

				q = AddTimeFilter(q, timeFilter);
				q = AddPVFilter(q, onlyWithPVs);

				return q.Count();

			} else {

				var directQ = Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					directQ = directQ.Where(s => songTypes.Contains(s.SongType));

				directQ = AddTimeFilter(directQ, timeFilter);
				directQ = AddPVFilter(directQ, onlyWithPVs);

				directQ = AddNameFilter(directQ, query, nameMatchMode, onlyByName);

				var direct = directQ.ToArray();

				var additionalNamesQ = Query<SongName>()
					.Where(m => !m.Song.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					additionalNamesQ = additionalNamesQ.Where(s => songTypes.Contains(s.Song.SongType));

				additionalNamesQ = AddTimeFilter(additionalNamesQ, timeFilter);
				additionalNamesQ = AddPVFilter(additionalNamesQ, onlyWithPVs);

				additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a))
					.ToArray();

				return direct.Count() + additionalNames.Count();

			}

		}

	}

}
