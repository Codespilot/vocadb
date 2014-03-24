using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class SongStrings {

		public SongStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; private set; }

		public string NewSongInfo {
			get { return ResourceManager.GetString("NewSongInfo"); }
		}

	}

}
