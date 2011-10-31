using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Album {

	public class TrackArtistSelectionsRow {

		public TrackArtistSelectionsRow() {
			ArtistSelections = new List<TrackArtistSelection>();
		}

		public TrackArtistSelectionsRow(SongInAlbumContract songInAlbum, IEnumerable<ArtistContract> artists)
			: this() {

			ArtistSelections = artists.Select(a => new TrackArtistSelection(a)).ToArray();
			DisplayName = songInAlbum.TrackNumber + ". " + songInAlbum.Song.Name;
			Id = songInAlbum.Id;
			Song = songInAlbum.Song;

		}

		public IList<TrackArtistSelection> ArtistSelections { get; set; }

		public string DisplayName { get; set; }

		public int Id { get; set; }

		public SongContract Song { get; set; }

	}

	public class TrackArtistSelection {

		public TrackArtistSelection() { }

		public TrackArtistSelection(ArtistContract artist) {
			Artist = artist;
		}

		public ArtistContract Artist { get; set; }

		public bool Selected { get; set; }

	}

}