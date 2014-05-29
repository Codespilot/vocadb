using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Search {

	public class SearchRouteParams {

		public int? artistId { get; set; }

		public ArtistType? artistType { get; set; }

		public DiscType? discType { get; set; }

		public string filter { get; set; }

		public bool? onlyWithPVs { get; set; }

		public EntryType? searchType { get; set; }

		public SongType? songType { get; set; }

		public object sort { get; set; }

		public string tag { get; set; }

	}

}