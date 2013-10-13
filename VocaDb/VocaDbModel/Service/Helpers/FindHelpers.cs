using System;
using System.Linq;
using NHibernate;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.Helpers {

	public static class FindHelpers {

		private const int MaxSearchWords = 6;

		/// <summary>
		/// Adds a filter for a list of names.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="nameFilter"></param>
		/// <param name="matchMode"></param>
		/// <returns></returns>
		public static IQueryable<T> AddEntryNameFilter<T>(IQueryable<T> query, string nameFilter, 
			NameMatchMode matchMode, string[] words = null)
			where T : LocalizedString {

			switch (GetMatchMode(nameFilter, matchMode)) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Value == nameFilter);

				case NameMatchMode.Partial:
					return query.Where(m => m.Value.Contains(nameFilter));

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Value.StartsWith(nameFilter));

				case NameMatchMode.Words:
					words = words ?? GetQueryWords(nameFilter);

					switch (words.Length) {
						case 1:
							query = query.Where(q => q.Value.Contains(words[0]));
							break;
						case 2:
							query = query.Where(q => q.Value.Contains(words[0]) && q.Value.Contains(words[1]));
							break;
						case 3:
							query = query.Where(q => q.Value.Contains(words[0]) && q.Value.Contains(words[1]) && q.Value.Contains(words[2]));
							break;
						case 4:
							query = query.Where(q => q.Value.Contains(words[0]) && q.Value.Contains(words[1]) && q.Value.Contains(words[2]) && q.Value.Contains(words[3]));
							break;
						case 5:
							query = query.Where(q => q.Value.Contains(words[0]) && q.Value.Contains(words[1]) && q.Value.Contains(words[2]) && q.Value.Contains(words[3]) && q.Value.Contains(words[4]));
							break;
						case 6:
							query = query.Where(q => q.Value.Contains(words[0]) && q.Value.Contains(words[1]) && q.Value.Contains(words[2]) && q.Value.Contains(words[3]) && q.Value.Contains(words[4]) && q.Value.Contains(words[5]));
							break;
					}
					return query;

			}

			return query;

		}

		public static IOrderedQueryable<T> AddNameOrder<T>(IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Names.SortNames.Japanese);
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Names.SortNames.English);
				default:
					return criteria.OrderBy(e => e.Names.SortNames.Romaji);
			}

		}

		public static IQueryOver<TRoot, TSubType> AddNameOrder<TRoot, TSubType>(IQueryOver<TRoot, TSubType> criteria, 
			ContentLanguagePreference languagePreference) where TSubType : IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.Japanese:
					return criteria.OrderBy(e => e.Names.SortNames.Japanese).Asc;
				case ContentLanguagePreference.English:
					return criteria.OrderBy(e => e.Names.SortNames.English).Asc;
				default:
					return criteria.OrderBy(e => e.Names.SortNames.Romaji).Asc;
			}

		}

		public static IQueryable<Tag> AddTagNameFilter(IQueryable<Tag> query, string name, NameMatchMode matchMode) {

			switch (GetMatchMode(name, matchMode)) {
				case NameMatchMode.Exact:
					return query.Where(m => m.Name == name);

				case NameMatchMode.StartsWith:
					return query.Where(m => m.Name.StartsWith(name));

				default:
					return query.Where(m => m.Name.Contains(name));
			}

		}

		/// <summary>
		/// Adds a filter for entry's SortName.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="criteria"></param>
		/// <param name="name"></param>
		/// <param name="matchMode"></param>
		/// <returns></returns>
		public static IQueryable<T> AddSortNameFilter<T>(IQueryable<T> criteria, string name, NameMatchMode matchMode)
			where T : IEntryWithNames {

			var mode = GetMatchMode(name, matchMode);

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
						|| s.Names.SortNames.Japanese.Contains(name));

			}

		}

		public static bool ExactMatch(string query, NameMatchMode matchMode) {
			return GetMatchMode(query, matchMode) == NameMatchMode.Exact;
		}

		public static NameMatchMode GetMatchMode(string query, NameMatchMode matchMode, NameMatchMode defaultMode = NameMatchMode.Exact) {

			if (matchMode != NameMatchMode.Auto)
				return matchMode;

			if (query.Length < 3 && defaultMode != NameMatchMode.Auto)
				return defaultMode;

			return NameMatchMode.Words;

		}

		/// <summary>
		/// Gets match mode and query for search.
		/// 
		/// Handles short query terms and wildcard searches ("*" in the end).
		/// Will only be applied if the current match mode is Auto.
		/// </summary>
		/// <param name="query">Text query. Can be null or empty.</param>
		/// <param name="matchMode">Current match mode. Will be set if something else besides Auto.</param>
		/// <param name="defaultMode">Default match mode to be used for normal queries.</param>
		/// <returns>Text query. Can be null or empty, if original query is.</returns>
		public static string GetMatchModeAndQueryForSearch(string query, ref NameMatchMode matchMode, NameMatchMode defaultMode = NameMatchMode.Words) {

			if (string.IsNullOrEmpty(query))
				return query;

			query = query.Trim();

			if (matchMode != NameMatchMode.Auto)
				return query;

			if (query.Length <= 2) {
				matchMode = NameMatchMode.StartsWith;
				return query;
			}

			if (query.Length > 1 && query.EndsWith("*")) {
				matchMode = NameMatchMode.StartsWith;
				return query.Substring(0, query.Length - 1);
			}

			matchMode = defaultMode;
			return query;

		}

		public static string[] GetQueryWords(string query) {

			return query
				.Split(new[] { ' ' }, MaxSearchWords, StringSplitOptions.RemoveEmptyEntries)
				.Distinct()
				.ToArray();

		}

	}

}
