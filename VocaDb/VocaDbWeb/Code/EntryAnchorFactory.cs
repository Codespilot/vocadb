using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Web.Code {

	public class EntryAnchorFactory : IEntryLinkFactory {

		private readonly string baseUrl;

		/// <summary>
		/// Initializes entry anchor factory.
		/// </summary>
		/// <param name="baseUrl">
		/// Base URL to the website. This will be added before the relative URL. Cannot be null. Can be empty.
		/// </param>
		public EntryAnchorFactory(string baseUrl = "/") {

			ParamIs.NotNull(() => baseUrl);

			this.baseUrl = baseUrl;

		}

		public string CreateEntryLink(EntryType entryType, int id, string name) {

			return string.Format("<a href=\"{0}{1}/Details/{2}\">{3}</a>",
				baseUrl, entryType, id, name);

		}

		public string CreateEntryLink(IEntryBase entry) {

			return CreateEntryLink(entry.EntryType, entry.Id, entry.DefaultName);

		}

	}
}