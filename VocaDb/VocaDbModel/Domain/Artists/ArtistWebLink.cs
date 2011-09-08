namespace VocaDb.Model.Domain.Artists {

	public class ArtistWebLink : WebLink {

		private Artist artist;

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

	}

}
