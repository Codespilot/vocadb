using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.Artists {

	public class ArtistSearch {

		private readonly ContentLanguagePreference languagePreference;

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private IQueryable<T> AddNameMatchFilter<T>(IQueryable<T> criteria, string name, NameMatchMode matchMode)
			where T : IEntryWithNames {

			return FindHelpers.AddSortNameFilter(criteria, name, matchMode);

		}

		private IQueryable<Artist> AddOrder(IQueryable<Artist> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case ArtistSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case ArtistSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case ArtistSortRule.AdditionDateAsc:
					return criteria.OrderBy(a => a.CreateDate);
			}

			return criteria;

		}

		private IQueryOver<Artist, Artist> AddOrder(IQueryOver<Artist, Artist> criteria, ArtistSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case ArtistSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case ArtistSortRule.AdditionDate:
					return criteria.OrderBy(a => a.CreateDate).Desc;
			}

			return criteria;

		}

		public ArtistSearch(ContentLanguagePreference languagePreference) {
			this.languagePreference = languagePreference;
		}

		public PartialFindResult<Artist> Find(ISession session, ArtistQueryParams queryParams) {

			var query = queryParams.Common.Query;
			var artistTypes = queryParams.ArtistTypes;
			var draftsOnly = queryParams.Common.DraftOnly;
			var sortRule = queryParams.SortRule;
			var start = queryParams.Paging.Start;
			var maxResults = queryParams.Paging.MaxEntries;
			var nameMatchMode = queryParams.Common.NameMatchMode;

			string originalQuery = query;

			if (string.IsNullOrWhiteSpace(query)) {

				bool filterByArtistType = artistTypes.Any();
				Artist art = null;

				var q = session.QueryOver(() => art)
					.Where(s => !s.Deleted);

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				if (filterByArtistType)
					q = q.WhereRestrictionOn(s => s.ArtistType).IsIn(artistTypes);

				q = AddOrder(q, sortRule, LanguagePreference);

				var artists = q
					.TransformUsing(new DistinctRootEntityResultTransformer())
					.Skip(start)
					.Take(maxResults)
					.List();

				var count = (queryParams.Paging.GetTotalCount ? GetArtistCount(session, queryParams) : 0);

				return new PartialFindResult<Artist>(artists.ToArray(), count, originalQuery, false);

			} else {

				query = query.Trim();

				// Note: Searching by SortNames can be disabled in the future because all names should be included in the Names list anyway.
				var directQ = session.Query<Artist>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				if (artistTypes.Any())
					directQ = directQ.Where(s => artistTypes.Contains(s.ArtistType));

				directQ = AddNameMatchFilter(directQ, query, queryParams.Common.NameMatchMode);
				directQ = AddOrder(directQ, sortRule, LanguagePreference);

				var direct = directQ
					.ToArray();

				Artist[] exactResults;

				if (queryParams.Common.MoveExactToTop && nameMatchMode != NameMatchMode.StartsWith && nameMatchMode != NameMatchMode.Exact) {
					
					var exactQ = session.Query<ArtistName>()
						.Where(m => !m.Artist.Deleted);

					if (draftsOnly)
						exactQ = exactQ.Where(a => a.Artist.Status == EntryStatus.Draft);

					exactQ = exactQ.FilterByArtistName(query, null, NameMatchMode.StartsWith);

					exactQ = exactQ.FilterByArtistType(artistTypes);

					exactResults = AddOrder(exactQ
						.Select(m => m.Artist), sortRule, LanguagePreference)
						.Distinct()
						.Take(maxResults)
						.ToArray();
				
				} else {
					exactResults = new Artist[] {};
				}

				var additionalNamesQ = session.Query<ArtistName>()
					.Where(m => !m.Artist.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Artist.Status == EntryStatus.Draft);

				additionalNamesQ = additionalNamesQ.FilterByArtistName(query, null, queryParams.Common.NameMatchMode);

				additionalNamesQ = additionalNamesQ.FilterByArtistType(artistTypes);

				var additionalNames = AddOrder(additionalNamesQ
					.Select(m => m.Artist), sortRule, LanguagePreference)
					.Distinct()
					.Take(start + maxResults)	// Note: this needs to be verified with paging
					//.FetchMany(s => s.Names)
					.ToArray();

				var entries = exactResults.Union(direct.Union(additionalNames))
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				var count = (queryParams.Paging.GetTotalCount ? GetArtistCount(session, queryParams) : 0);

				return new PartialFindResult<Artist>(entries.ToArray(), count, originalQuery, exactResults.Any());

			}

		}

		private int GetArtistCount(ISession session, ArtistQueryParams queryParams) {

			var query = queryParams.Common.Query;
			var artistTypes = queryParams.ArtistTypes;
			var draftsOnly = queryParams.Common.DraftOnly;
			var nameMatchMode = queryParams.Common.NameMatchMode;

			if (string.IsNullOrWhiteSpace(query)) {

				var q = session.Query<Artist>()
					.Where(s =>
						!s.Deleted
						&& (!artistTypes.Any() || artistTypes.Contains(s.ArtistType)));

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				return q.Count();

			}

			query = query.Trim();

			var directQ = session.Query<Artist>()
				.Where(s => !s.Deleted);

			if (artistTypes.Any())
				directQ = directQ.Where(s => artistTypes.Contains(s.ArtistType));

			if (draftsOnly)
				directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

			directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

			var direct = directQ.ToArray();

			var additionalNamesQ = session.Query<ArtistName>()
				.Where(m => !m.Artist.Deleted);

			if (draftsOnly)
				additionalNamesQ = additionalNamesQ.Where(a => a.Artist.Status == EntryStatus.Draft);

			additionalNamesQ = additionalNamesQ.FilterByArtistName(query, null, nameMatchMode);

			additionalNamesQ = additionalNamesQ.FilterByArtistType(artistTypes);

			var additionalNames = additionalNamesQ
				.Select(m => m.Artist)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

	}
}
