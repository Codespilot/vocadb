using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Web.Code {

	public class EntryAnchorFactory : IEntryLinkFactory {

		/*private readonly UrlHelper url;

		public EntryAnchorFactory(UrlHelper url) {

			ParamIs.NotNull(() => url);

			this.url = url;

		}

		public string CreateEntryLink(IEntryBase entry) {
			
			return string.Format("<a href=\"{0}\">{1}</a>",
				url.Action("Details", entry.EntryType.ToString(), new { id = entry.Id }),
				entry.DefaultName);


		}*/

		public string CreateEntryLink(IEntryBase entry) {
			
			return string.Format("<a href=\"{0}\">{1}</a>",
				string.Format("/{0}/Details/{1}", entry.EntryType, entry.Id),
				entry.DefaultName);

		}

	}
}