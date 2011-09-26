using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumName : LocalizedStringWithId {

		private Album album;

		public AlbumName() { }

		public AlbumName(Album album, LocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

	}

}
