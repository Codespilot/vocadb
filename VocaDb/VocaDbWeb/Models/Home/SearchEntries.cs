using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.Home {

	public class SearchEntries {

		public SearchEntries() { }

		public SearchEntries(string filter,
			PartialFindResult<AlbumWithAdditionalNamesContract> albums, PartialFindResult<ArtistWithAdditionalNamesContract> artists,
			PartialFindResult<SongWithAdditionalNamesContract> songs) {

			Filter = filter;
			Albums = albums;
			Artists = artists;
			Songs = songs;

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> Albums { get; set; }

		public PartialFindResult<ArtistWithAdditionalNamesContract> Artists { get; set; }

		public string Filter { get; set; }

		public PartialFindResult<SongWithAdditionalNamesContract> Songs { get; set; }

	}

}