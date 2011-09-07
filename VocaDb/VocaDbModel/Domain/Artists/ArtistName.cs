using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistName : LocalizedString {

		private Artist artist;

		public Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

	}

}
