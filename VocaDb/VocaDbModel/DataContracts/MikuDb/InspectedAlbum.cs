using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.DataContracts.MikuDb {

	public class InspectedAlbum {

		public AlbumWithAdditionalNamesContract ExistingAlbum { get; set; }

		public MikuDbAlbumContract ImportedAlbum { get; set; }

		public string[] MissingArtists { get; set; }

		public InspectedTrack[] Tracks { get; set; }

	}

	public class InspectedTrack {

		public SongWithAdditionalNamesContract ExistingSong { get; set; }

		public ImportedAlbumTrack ImportedTrack { get; set; }

	}

}
