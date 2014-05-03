using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.Helpers {

	public static class EntryWithNamesQueryableExtender {

		public static IQueryable<EntryIdAndName> SelectIdAndName<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference)
			where T: class, IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.English:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.English, Id = a.Id });
				case ContentLanguagePreference.Romaji:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Romaji, Id = a.Id });
				case ContentLanguagePreference.Japanese:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id });
				default:
					return query.Select(a => new EntryIdAndName { Name = a.Names.SortNames.Japanese, Id = a.Id });
			}

		}

		public static IQueryable<EntryBaseContract> SelectEntryBase<T>(this IQueryable<T> query, ContentLanguagePreference languagePreference, EntryType entryType)
			where T: class, IEntryWithNames {

			switch (languagePreference) {
				case ContentLanguagePreference.English:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.English, Id = a.Id, EntryType = entryType });
				case ContentLanguagePreference.Romaji:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Romaji, Id = a.Id, EntryType = entryType });
				case ContentLanguagePreference.Japanese:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType });
				default:
					return query.Select(a => new EntryBaseContract { DefaultName = a.Names.SortNames.Japanese, Id = a.Id, EntryType = entryType });
			}

		}

	}
}
