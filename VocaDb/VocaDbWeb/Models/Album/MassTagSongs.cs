using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Album {

	public class MassTagSongs {

		public MassTagSongs() {
			Tracks = new List<TrackArtistSelectionsRow>();
		}

		public MassTagSongs(AlbumDetailsContract contract)
			: this() {

			Artists = contract.ArtistLinks.Select(a => a.Artist).ToArray();
			Id = contract.Id;
			Name = contract.Name;
			Tracks = contract.Songs.Select(s => new TrackArtistSelectionsRow(s, Artists)).ToArray();

		}

		public ArtistContract[] Artists { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public IList<TrackArtistSelectionsRow> Tracks { get; set; }

	}

}