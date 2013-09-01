using System;
using System.IO;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.TestSupport {

	public class TempImagePathMapper : IImagePathMapper {

		public TempImagePathMapper() {
			Folder = Path.GetTempPath() + "\\VocaDB";
		}

		public string Folder { get; set; }

		public string GetImagePath(EntryType entryType, string fileName) {

			return string.Format("{0}/{1}/{2}", Folder, entryType.ToString(), fileName);

		}

		public string GetImageUrlAbsolute(EntryType entryType, string fileName) {
			throw new NotImplementedException();
		}

	}

}
