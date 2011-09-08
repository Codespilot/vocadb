using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistName : LocalizedStringWithId {

		private Artist artist;

		public ArtistName() {}

		public ArtistName(Artist artist, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language) {

			Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

	}

}
