using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.DataContracts.MikuDb {

	public class InspectedAlbum {

		public InspectedAlbum() { }

		public InspectedAlbum(MikuDbAlbumContract importedAlbum) {
			ImportedAlbum = importedAlbum;
		}

		public InspectedArtist[] Artists { get; set; }

		public AlbumWithAdditionalNamesContract ExistingAlbum { get; set; }

		public MikuDbAlbumContract ImportedAlbum { get; set; }

		public InspectedTrack[] Tracks { get; set; }

	}

	public class InspectedArtist {

		public InspectedArtist() { }

		public InspectedArtist(string artistName) {
			Name = artistName;
		}

		public ArtistWithAdditionalNamesContract ExistingArtist { get; set; }

		public string Name { get; set; }

	}

	public class InspectedTrack {

		public InspectedTrack() { }

		public InspectedTrack(ImportedAlbumTrack importedTrack) {
			ImportedTrack = importedTrack;
		}

		public SongWithAdditionalNamesContract ExistingSong { get; set; }

		public ImportedAlbumTrack ImportedTrack { get; set; }

	}

}
