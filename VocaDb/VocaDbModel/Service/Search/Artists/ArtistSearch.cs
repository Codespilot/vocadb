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
using VocaDb.Model.Service.Search.SongSearch;

namespace VocaDb.Model.Service.Search.Artists {

	public class ArtistSearch {

		private readonly ContentLanguagePreference languagePreference;
		private readonly IQuerySource querySource;

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

				var additionalNamesQ = session.Query<ArtistName>()
					.Where(m => !m.Artist.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Artist.Status == EntryStatus.Draft);

				additionalNamesQ = additionalNamesQ.FilterByArtistName(query, null, queryParams.Common.NameMatchMode);

				additionalNamesQ = additionalNamesQ.FilterByArtistType(artistTypes);

				var additionalNames = AddOrder(additionalNamesQ
					.Select(m => m.Artist), sortRule, LanguagePreference)
					.Distinct()
					.Take(maxResults)
					//.FetchMany(s => s.Names)
					.ToArray()
					.Where(a => !direct.Contains(a));

				var entries = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				bool foundExactMatch = false;

				if (queryParams.Common.MoveExactToTop) {

					var exactMatch = entries
						.Where(e => e.Names.Any(n => n.Value.StartsWith(query, StringComparison.InvariantCultureIgnoreCase)))
						.ToArray();

					if (exactMatch.Any()) {
						entries = CollectionHelper.MoveToTop(entries, exactMatch).ToArray();
						foundExactMatch = true;
					}

				}

				var count = (queryParams.Paging.GetTotalCount ? GetArtistCount(session, queryParams) : 0);

				return new PartialFindResult<Artist>(entries.ToArray(), count, originalQuery, foundExactMatch);

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
