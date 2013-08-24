using System;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	public class AlbumSearch {

		private readonly ContentLanguagePreference languagePreference;
		private readonly IQuerySource querySource;

		private ContentLanguagePreference LanguagePreference {
			get { return languagePreference; }
		}

		private IQueryable<ArtistForAlbum> AddArtistParticipationStatus(IQueryable<ArtistForAlbum> query, int artistId, ArtistAlbumParticipationStatus participation) {

			if (participation == ArtistAlbumParticipationStatus.Everything || artistId == 0)
				return query;

			var artist = querySource.Load<Artist>(artistId);
			var musicProducerTypes = new[] {ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup};

			if (musicProducerTypes.Contains(artist.ArtistType)) {

				//var producerRoles = new[] {ArtistRoles.Default, ArtistRoles.Composer, ArtistRoles.Arranger};
				var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(a => !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Album.ArtistString.Default != ArtistHelper.VariousArtists);
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(a => a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Album.ArtistString.Default == ArtistHelper.VariousArtists);
					default:
						return query;
				}

			} else {

				switch (participation) {
					case ArtistAlbumParticipationStatus.OnlyMainAlbums:
						return query.Where(a => !a.IsSupport);
					case ArtistAlbumParticipationStatus.OnlyCollaborations:
						return query.Where(a => a.IsSupport);
					default:
						return query;
				}
				
			}

		}

		private IQueryable<Album> AddDiscTypeRestriction(IQueryable<Album> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.DiscType == discType) : query);
		}

		private IQueryable<AlbumName> AddDiscTypeRestriction(IQueryable<AlbumName> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<AlbumTagUsage> AddDiscTypeRestriction(IQueryable<AlbumTagUsage> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<ArtistForAlbum> AddDiscTypeRestriction(IQueryable<ArtistForAlbum> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<Album> AddNameMatchFilter(IQueryable<Album> criteria, string name, NameMatchMode matchMode) {

			var mode = FindHelpers.GetMatchMode(name, matchMode);

			if (mode == NameMatchMode.Exact) {

				return criteria.Where(s =>
					s.Names.SortNames.English == name
						|| s.Names.SortNames.Romaji == name
						|| s.Names.SortNames.Japanese == name);

			} else if (mode == NameMatchMode.StartsWith) {

				return criteria.Where(s =>
				                      s.Names.SortNames.English.StartsWith(name)
				                      || s.Names.SortNames.Romaji.StartsWith(name)
				                      || s.Names.SortNames.Japanese.StartsWith(name));

			} else {

				return criteria.Where(s =>
					s.Names.SortNames.English.Contains(name)
						|| s.Names.SortNames.Romaji.Contains(name)
						|| s.Names.SortNames.Japanese.Contains(name)
						|| (s.OriginalRelease.CatNum != null && s.OriginalRelease.CatNum.Contains(name)));

			}

		}

		private IQueryable<Album> AddOrder(IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.ReleaseDate:
					return AddReleaseRestriction(criteria)
						.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.RatingTotal:
					return criteria.OrderByDescending(a => a.RatingTotal)
						.ThenByDescending(a => a.RatingAverageInt);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;

		}

		private IQueryable<Album> AddReleaseRestriction(IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

		private IQueryable<T> Query<T>() {
			return querySource.Query<T>();
		}

		public AlbumSearch(IQuerySource querySource, ContentLanguagePreference languagePreference) {
			this.querySource = querySource;
			this.languagePreference = languagePreference;
		}

		public PartialFindResult<Album> Find(AlbumQueryParams queryParams) {

			var query = queryParams.Common.Query ?? string.Empty;
			var discType = queryParams.AlbumType;
			var start = queryParams.Paging.Start;
			var maxResults = queryParams.Paging.MaxEntries;
			var draftsOnly = queryParams.Common.DraftOnly;
			var getTotalCount = queryParams.Paging.GetTotalCount;
			var nameMatchMode = queryParams.Common.NameMatchMode;
			var sortRule = queryParams.SortRule;
			var moveExactToTop = queryParams.Common.MoveExactToTop;

			Album[] entries;
			string originalQuery = query;
			bool foundExactMatch = false;

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var albumsQ = Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumsQ = albumsQ.Where(a => a.Status == EntryStatus.Draft);

				albumsQ = AddDiscTypeRestriction(albumsQ, discType);

				albumsQ = AddOrder(albumsQ, sortRule, LanguagePreference);

				entries = albumsQ
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				// TODO: refactor using advanced search parser
			} else if (query.StartsWith("tag:")) {

				var tagName = query.Substring(4);

				var tagQ = Query<AlbumTagUsage>()
					.Where(m => !m.Album.Deleted && m.Tag.Name == tagName);

				if (draftsOnly)
					tagQ = tagQ.Where(a => a.Album.Status == EntryStatus.Draft);

				tagQ = AddDiscTypeRestriction(tagQ, discType);
				entries = AddOrder(tagQ.Select(m => m.Album), sortRule, LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				// TODO: refactor using advanced search parser
			} else if (queryParams.ArtistId != 0 || query.StartsWith("artist:")) {

 				int artistId;
				if (queryParams.ArtistId != 0)
					artistId = queryParams.ArtistId;
				else
					int.TryParse(query.Substring(7), out artistId);

				var albumQ = Query<ArtistForAlbum>()
					.Where(m => !m.Album.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Album.Status == EntryStatus.Draft);

				albumQ = AddDiscTypeRestriction(albumQ, discType);
				albumQ = AddArtistParticipationStatus(albumQ, artistId, queryParams.ArtistParticipationStatus);

				entries = AddOrder(albumQ.Select(m => m.Album), sortRule, LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else {

				query = query.Trim();

				// Searching by SortNames can be disabled in the future because all names should be included in the Names list anyway.
				var directQ = Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				directQ = AddDiscTypeRestriction(directQ, discType);
				directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

				var direct = AddOrder(directQ, sortRule, LanguagePreference)
					.ToArray();

				var additionalNamesQ = Query<AlbumName>()
					.Where(m => !m.Album.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

				additionalNamesQ = AddDiscTypeRestriction(additionalNamesQ, discType);

				additionalNamesQ = additionalNamesQ.AddEntryNameFilter(query, nameMatchMode);

				var additionalNames =
					AddOrder(additionalNamesQ.Select(m => m.Album), sortRule, LanguagePreference)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a));

				entries = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				if (moveExactToTop) {

					var exactMatch = entries
						.Where(e => e.Names.Any(n => n.Value.StartsWith(query, StringComparison.InvariantCultureIgnoreCase)))
						.ToArray();

					if (exactMatch.Any()) {
						entries = CollectionHelper.MoveToTop(entries, exactMatch).ToArray();
						foundExactMatch = true;
					}

				}

			}

			var count = (getTotalCount ? GetAlbumCount(queryParams, query, discType, draftsOnly, nameMatchMode, sortRule) : 0);

			return new PartialFindResult<Album>(entries, count, originalQuery, foundExactMatch);


		}

		public int GetAlbumCount(
			AlbumQueryParams queryParams, string query, DiscType discType, bool draftsOnly, NameMatchMode nameMatchMode, AlbumSortRule sortRule) {

			query = query ?? string.Empty;

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var albumQ = Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Status == EntryStatus.Draft);

				if (sortRule == AlbumSortRule.ReleaseDate)
					albumQ = AddReleaseRestriction(albumQ);

				albumQ = AddDiscTypeRestriction(albumQ, discType);

				return albumQ.Count();

			}

			if (query.StartsWith("tag:")) {

				var tagName = query.Substring(4);

				var tagQ = Query<AlbumTagUsage>()
					.Where(m => !m.Album.Deleted && m.Tag.Name == tagName);

				if (draftsOnly)
					tagQ = tagQ.Where(a => a.Album.Status == EntryStatus.Draft);

				tagQ = AddDiscTypeRestriction(tagQ, discType);

				return tagQ.Count();

			}

			if (queryParams.ArtistId != 0 || query.StartsWith("artist:")) {

				int artistId;
				if (queryParams.ArtistId != 0)
					artistId = queryParams.ArtistId;
				else
					int.TryParse(query.Substring(7), out artistId);

				var albumQ = Query<ArtistForAlbum>()
					.Where(m => !m.Album.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Album.Status == EntryStatus.Draft);

				albumQ = AddDiscTypeRestriction(albumQ, discType);
				albumQ = AddArtistParticipationStatus(albumQ, artistId, queryParams.ArtistParticipationStatus);
				return albumQ.Count();

			}

			var directQ = Query<Album>()
				.Where(s => !s.Deleted);

			if (draftsOnly)
				directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

			if (sortRule == AlbumSortRule.ReleaseDate)
				directQ = AddReleaseRestriction(directQ);

			directQ = AddDiscTypeRestriction(directQ, discType);

			directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

			var direct = directQ.ToArray();

			var additionalNamesQ = Query<AlbumName>()
				.Where(m => !m.Album.Deleted);

			if (draftsOnly)
				additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

			additionalNamesQ = AddDiscTypeRestriction(additionalNamesQ, discType);

			additionalNamesQ = additionalNamesQ.AddEntryNameFilter(query, nameMatchMode);

			var additionalNamesAlbumQ = additionalNamesQ.Select(a => a.Album);

			if (sortRule == AlbumSortRule.ReleaseDate)
				additionalNamesAlbumQ = AddReleaseRestriction(additionalNamesAlbumQ);

			var additionalNames = additionalNamesAlbumQ
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

	}

}
