using System.Web;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Images {

	public class ServerImagePathMapper : IImagePathMapper {

		public string GetImagePath(EntryType entryType, string fileName) {
			return HttpContext.Current.Server.MapPath(string.Format("~\\EntryImg\\{0}\\{1}", entryType, fileName));
		}

		public string GetImageUrlAbsolute(EntryType entryType, string fileName) {
			return string.Format("{0}/EntryImg/{1}/{2}", AppConfig.HostAddress, entryType, fileName);
		}

	}
}
