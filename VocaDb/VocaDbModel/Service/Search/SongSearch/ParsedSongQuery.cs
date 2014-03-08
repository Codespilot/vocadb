namespace VocaDb.Model.Service.Search.SongSearch {

	public class ParsedSongQuery {

		public string ArtistTag { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public string TagName { get; set; }

		public bool HasNameQuery {
			get {
				return !string.IsNullOrEmpty(Name);
			}
		}

	}

}
