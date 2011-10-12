namespace VocaDb.Model.Domain.Albums {

	public class AlbumWebLink : WebLink {

		private Album album;

		public AlbumWebLink() { }

		public AlbumWebLink(Album album, string description, string url)
			: base(description, url) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public override string ToString() {
			return base.ToString() + " for " + Album;
		}

	}

}
