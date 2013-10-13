using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.Helpers {

	public static class TagQueryableExtender {

		public static IQueryable<Tag> AddTagNameFilter(this IQueryable<Tag> query, string nameFilter, NameMatchMode matchMode) {

			return FindHelpers.AddTagNameFilter(query, nameFilter, matchMode);

		}

	}
}
